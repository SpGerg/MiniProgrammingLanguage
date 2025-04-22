type player
    last_broadcast
    function broadcast(content)
end

implement function player.broadcast(content)
    self.last_broadcast = content
    print(content)
end

instance = create player
instance.broadcast("Hello, world")

print(instance)