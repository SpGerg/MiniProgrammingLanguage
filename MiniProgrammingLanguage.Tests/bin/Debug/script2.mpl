type Player
    @sharp_kit_ignore_case
    bindable name
    @sharp_kit_ignore_case
    bindable function broadcast(content)
end

player_class = get_type("MiniProgrammingLanguage.Tests.Player")

bind(typeof("Player"), player_class)

player = create Player

for i in 1000000
    player.broadcast(i)
end