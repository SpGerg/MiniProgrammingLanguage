type test
    @sharp_kit_ignore_case
    bindable name
    @sharp_kit_ignore_case
    bindable function execute(argument)
end

instance = create_type("MiniProgrammingLanguage.Tests.TestProject")

result = create_based_on(create test, instance)
result.name = "Hello, world"

other = result.execute("Yes")

print(other)