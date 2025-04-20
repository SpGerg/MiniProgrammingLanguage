type player
    name: string
end

function create_player(name)
    pl = create player
    pl.name = name

    return pl
end