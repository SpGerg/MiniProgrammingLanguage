__REQUIRE_DEPENDENCY("Exiled.Events")
__REQUIRE_MODULE("exiled_api")

castable type event_arguments
    player_left_event_arguments left
end

type listener
    constructor(listeners)
    list<(event_arguments arguments) -> () listeners
    function subscribe(event_arguments arguments)
    function invoke()
    function unsubscribe(event_arguments arguments)
end

type event
    listener listener
end

//listeners = create listener([ ... ])
implement function listener.constructor(listeners)
    self = create listener
    self.listeners = create listener

    self.listeners.add_range(listeners)

    return self
end

implement function listener.subscribe((event_arguments arguments) -> () func)
    self.listeners.add(func)
end

implement function listener.invoke((event_arguments arguments) -> () func)
    for listener in self.listeners
        listener(arguments)
    end
end

implement function listener.unsubscribe((event_arguments arguments) -> () func)
    self.listeners.remove(func)
end

---

type player_left_event_arguments
    player player
end

events.left.invoke((event_arguments) arguments)

function show_broadcast_on_left(player_left_event_arguments arguments)
    exiled_api.map.broadcast("Player left")
end

events.left.subscribe(show_broadcast_on_left)