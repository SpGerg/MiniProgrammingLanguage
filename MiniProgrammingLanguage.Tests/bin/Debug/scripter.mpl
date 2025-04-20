type player
    name: string
end

function crate_player(name)
    pl = create player
    pl.name = name

    return pl
end