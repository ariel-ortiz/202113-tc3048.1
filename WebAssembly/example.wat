;; Simple WebAssembly example.

(module
  (func
    ;; Function signature: i32 duplicate(i32 $x)
    (export "duplicate")
    (param $x i32)
    (result i32)

    ;; Body of the function
    local.get $x
    i32.const 2
    i32.mul
  )

  (func
    (export "f2c")
    (param $f f32)
    (result f32)
    f32.const 5.0
    local.get $f
    f32.const 32.0
    f32.sub ;; $f - 32.0
    f32.mul ;; 5.0 * ($f - 32.0)
    f32.const 9.0
    f32.div ;; 5.0 * ($f - 32.0) / 9.0
  )

  (func
    (export "root")
    (param $a f64)
    (param $b f64)
    (param $c f64)
    (result f64)
    local.get $b
    local.get $b
    f64.mul
    f64.const 4.0
    local.get $a
    f64.mul
    local.get $c
    f64.mul
    f64.sub
    f64.sqrt
    local.get $b
    f64.sub
    f64.const 2.0
    local.get $a
    f64.mul
    f64.div
  )
)
