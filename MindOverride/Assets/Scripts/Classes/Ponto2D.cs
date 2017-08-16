public class ponto2D
{
    public int x;
    public int y;

    public ponto2D(int newX, int newY)
    {
        this.x = newX;
        this.y = newY;
    }

    /**
    /* @name: isEqual
    /* @version: 1.0
    /* @Descrition: True caso as coordenadas sejam iguais
    */
    public bool isEqual(ponto2D ponto)
    {
        return ((this.x == ponto.x) && (this.y == ponto.y));
    }

    /**
    /* @name: distancia
    /* @version: 1.0
    /* @Descrition: Retorna a distancia deste até ponto, sem considerar diagonais.
    */
    public int distancia(ponto2D ponto)
    {
        int distancia = 0;

        if (this.isEqual(ponto))
        {
            return distancia;
        }

        if (this.x > ponto.x)
        {
            distancia += this.x - ponto.x;
        }
        else
        {
            distancia += ponto.x - this.x;
        }
        if (this.y > ponto.y)
        {
            distancia += this.y - ponto.y;
        }
        else
        {
            distancia += ponto.y - this.y;
        }

        return distancia;
    }

    public string toString()
    {
        return ("("+this.x+","+this.y+")");
    }
}
