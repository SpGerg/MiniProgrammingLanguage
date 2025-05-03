type Item
    bindable Type
    @sharp_kit_ignore_case
    bindable function destroy()
    @sharp_kit_ignore_case
    bindable function execute()
    @sharp_kit_ignore_case
    bindable function createExecutable()
    @sharp_kit_ignore_case
    bindable function pepe()
end

item_class = get_type("MiniProgrammingLanguage.Tests.Item")
executable_item_class = get_type("MiniProgrammingLanguage.Tests.ExecutableItem")
other_item_class = get_type("MiniProgrammingLanguage.Tests.OtherItem")

bind(typeof("Item"), item_class)
bind(typeof("Item"), executable_item_class)
bind(typeof("Item"), other_item_class)

item = create_from_extender(typeof("Item"), executable_item_class)
item.execute()

g = item.Type

print(g is ItemType.Executable)