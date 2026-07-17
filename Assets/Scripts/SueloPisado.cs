// Tipo de suelo pisado
public enum DondeEsta
{
    Suelo,
    Aire,
    Pared,
    Agua
}
public enum ParedContacto
{
    Pared,
    Aire
}
public enum DireccionPersonaje
{
    Izquierda,
    Derecha
}
public enum MovimientoHoriz
{
    Quieto,
    Avanzando,
    Dash
}
public enum MovimientoVert
{
    Quieto, 
    SaltoCaida
}
public enum Combate
{
    Quieto,
    Ataque,
    AtaqueAereo
}
public enum Estado
{
    Normal,
    Invulnerable,
    Muerto
}
public enum Efectos
{
    Normal,
    Relentizado
}