/*

Original BNF Grammar:

    Expr ::= Expr "+" Term
    Expr ::= Term
    Term ::= Term "*" Fact
    Term ::= Fact
    Fact ::= "int"
    Fact ::= "(" Expr ")"

LL(1) Grammar

    (0) Prog ::= Expr "EOF"
    (1) Expr ::= Term ("+" Term)*
    (2) Term ::= Fact ("*" Fact)*
    (3) Fact ::= "int" | "(" Expr ")"

*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenCategory {
    INT, PLUS, TIMES, OPEN_PAR, CLOSE_PAR, EOF, BAD_TOKEN
}

public class Token {
    public TokenCategory Category { get; }
    public String Lexeme { get; }

    public Token(TokenCategory category, String lexeme) {
        Category = category;
        Lexeme = lexeme;
    }

    public override String ToString() {
        return $"[{Category}, \"{Lexeme}\"]";
    }
}

public class Scanner {
    readonly String input;
    static readonly Regex regex = new Regex(
        @"(\d+)|([+])|([*])|([(])|([)])|(\s)|(.)");

    public Scanner(String input) {
        this.input = input;
    }

    public IEnumerable<Token> Scan() {
        var result = new LinkedList<Token>();

        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Success) {
                result.AddLast(new Token(TokenCategory.INT, m.Value));
            } else if (m.Groups[2].Success) {
                result.AddLast(new Token(TokenCategory.PLUS, m.Value));
            } else if (m.Groups[3].Success) {
                result.AddLast(new Token(TokenCategory.TIMES, m.Value));
            } else if (m.Groups[4].Success) {
                result.AddLast(new Token(TokenCategory.OPEN_PAR, m.Value));
            } else if (m.Groups[5].Success) {
                result.AddLast(new Token(TokenCategory.CLOSE_PAR, m.Value));
            } else if (m.Groups[6].Success) {
                // skip
            } else if (m.Groups[7].Success) {
                result.AddLast(new Token(TokenCategory.BAD_TOKEN, m.Value));
            }
        }
        result.AddLast(new Token(TokenCategory.EOF, null));

        return result;
    }
}

public class SyntaxError: Exception {}

public class Parser {
    IEnumerator<Token> tokenStream;

    public Parser(IEnumerator<Token> tokenStream) {
        this.tokenStream = tokenStream;
        this.tokenStream.MoveNext();
    }

    public TokenCategory Current {
        get {
            return tokenStream.Current.Category;
        }
    }

    public Token Expect(TokenCategory category) {
        if (Current == category) {
            Token current = tokenStream.Current;
            tokenStream.MoveNext();
            return current;
        } else {
            throw new SyntaxError();
        }
    }

    // (0)
    public void Prog() {
        Expr();
        Expect(TokenCategory.EOF);
    }

    // (1)
    public void Expr() {
        Term();
        while (Current == TokenCategory.PLUS) {
            Expect(TokenCategory.PLUS);
            Term();
        }
    }

    // (2)
    public void Term() {
        Fact();
        while (Current == TokenCategory.TIMES) {
            Expect(TokenCategory.TIMES);
            Fact();
        }
    }

    // (3)
    public void Fact() {
        switch (Current) {
            case TokenCategory.INT:
                Expect(TokenCategory.INT);
                break;
            case TokenCategory.OPEN_PAR:
                Expect(TokenCategory.OPEN_PAR);
                Expr();
                Expect(TokenCategory.CLOSE_PAR);
                break;
            default:
                throw new SyntaxError();
        }
    }
}

public class Driver {
    public static void Main() {
        Console.Write("> ");
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Scan().GetEnumerator());
        try {
            parser.Prog();
            Console.WriteLine("Syntax OK!");
        } catch (SyntaxError) {
            Console.WriteLine("Bad syntax!");
        }
    }
}
