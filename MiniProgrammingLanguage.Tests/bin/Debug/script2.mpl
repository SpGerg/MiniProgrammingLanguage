enum test
    left = 0
    right = 1
    forward = 2
    bottom = 3
end

enum other_test
    left = 0
    right = 1
    forward = 2
    bottom = 3
end

function checker(arg: enum_member)
    print(arg)
end

checker(test.left)