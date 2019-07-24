using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularAutomata : MonoBehaviour
{
    [Header("Tiles")] [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile tileVivante;
    [SerializeField] private Tile tileMorte;

    /*RAJOUTER APRÈS COURS*/
    struct Case //Contient les informations propres aux cases
    {
        public Case[] voisins; //Neighbors
        public bool estVivant; //Is alive?

        public bool etatFutur; //Future state, used when generating 

    }

    Case[,] cases = new Case[50, 50];

    /*RAJOUTER APRÈS COURS*/

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    void Generate()
    {
        for (int i = 0; i < 50; i++) {
            for (int j = 0; j < 50; j++) {
                Tile tile = tileVivante;

                int random = Random.Range(0, 2);

                if (random == 0) {
                    tile = tileVivante;
                } else {
                    tile = tileMorte;
                }

                tilemap.SetTile(new Vector3Int(i, j, 0), tile);
                /*RAJOUTER APRÈS COURS*/
                //Enregistré si la case est vivante
                if (random == 0) {
                    cases[i, j].estVivant = true;
                } else {
                    cases[i, j].estVivant = false;
                }

                /*RAJOUTER APRÈS COURS*/
            }
        }

        /*RAJOUTER APRÈS COURS*/
        StartCoroutine(Iterate());
        /*RAJOUTER APRÈS COURS*/
    }

    /*RAJOUTER APRÈS COURS*/
    IEnumerator Iterate()
    {
        for (int iteration = 0; iteration < 10; iteration++) {
            for (int i = 0; i < 50; i++) {
                for (int j = 0; j < 50; j++) {

                    BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

                    int voisinVivant = 0;

                    foreach (Vector2Int b in bounds.allPositionsWithin) {
                        if (b.x == 0 && b.y == 0) {
                            continue; //Permet d'ignorer cette tile si c'est elle même
                        }

                        if (i + b.x < 0 || i + b.x >= 50) {
                            continue; //Permet d'ignorer cette tile puisque sa position x est en dehors de la tilemap
                        }

                        if (j + b.y < 0 || j + b.y >= 50) {
                            continue; //Permet d'ignorer cette tile puisque sa position y est en dehors de la tilemap
                        }

                        if (cases[i + b.x, j + b.y].estVivant) {
                            voisinVivant++; //Permet de faire + 1 au nombre de voisin vivant
                        }
                    }

                    if (cases[i, j].estVivant && (voisinVivant == 1 || voisinVivant >= 4)) {
                        //Si vivant est 1 ou 4 et + de voisins vivant, reste vivant
                        cases[i, j].etatFutur = true;
                    } else if (!cases[i, j].estVivant && voisinVivant >= 5) {
                        //Si mort et 5 voisin ou plus vivant, alors devient vivant
                        cases[i, j].etatFutur = true;
                    } else { //Sinon meurt
                        cases[i, j].etatFutur = false;
                    }
                }
            }

            //Mets à jour depuis l'état futur à l'état actuel
            for (int i = 0; i < 50; i++) {
                for (int j = 0; j < 50; j++) {
                    cases[i, j].estVivant = cases[i, j].etatFutur;
                    if (cases[i, j].estVivant) {
                        tilemap.SetTile(new Vector3Int(i, j, 0), tileVivante);
                    } else {
                        tilemap.SetTile(new Vector3Int(i, j, 0), tileMorte);
                    }

                }
                yield return new WaitForEndOfFrame();
            }

            //Mets cette fonction en pause pendant 1 secondes
            yield return new WaitForSeconds(1);
        }
    }
    /*RAJOUTER APRÈS COURS*/
}
