#!/usr/bin/env python

# Generic Wat execution script.
# Copyright (C) 2021 Ariel Ortiz, ITESM CEM
#
# To run, at the terminal type:
#
#     python execute.py input_file.wat

from sys import argv
from wasmer import engine, Module, wat2wasm, Store, Instance
from wasmer_compiler_cranelift import Compiler

def create_instance(file_name):
    with open(file_name) as wat_file:
        return Instance(Module(Store(engine.JIT(Compiler)),
                               wat2wasm(wat_file.read())))

def main():
    if len(argv) != 2:
        raise SystemExit('Please specify the name of the input Wat file.')

    instance = create_instance(argv[1])

    # Run exported function, print result
    print(instance.exports.duplicate(21))
    print(instance.exports.f2c(212.0))
    print(instance.exports.f2c(32.0))
    print(instance.exports.f2c(-40.0))
    print(instance.exports.root(2.0, 4.0, 2.0))
    print(instance.exports.root(1.0, 1.0, 0.0))
    print(instance.exports.root(4.0, 5.0, 1.0))

main()
