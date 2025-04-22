type broadcaster
    function broadcast(content)
end

implement function broadcaster.broadcast(content)
    print(content)
end

broadcaster_instance = create broadcaster

for i in 1000000
    print((string) i)
end