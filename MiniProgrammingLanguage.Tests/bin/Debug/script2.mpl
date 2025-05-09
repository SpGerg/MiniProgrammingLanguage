saver = create saver
saver.filepath = "C:\Users\spger\AppData\Roaming\EXILED\Configs\Plugins\mpl\7777.yml\scripts\"

plugin = xp_system_plugin

function update_level(player, level)
    saver.set(player.get_user_id(), 0)
    player.rank = "Level | " + (string) level
end

function set_level_on_verified(player)
    update_level(player, 0)
end

plugin.on_enabled.subscribe(function()
    on_verified.subscribe(set_level_on_verified)
end)

plugin.on_disabled.unsubscribe(function()
    on_verified.unsubscribe(set_level_on_verified)
end)