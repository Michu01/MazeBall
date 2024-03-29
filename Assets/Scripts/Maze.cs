using Assets;

using System;
using System.Linq;

using UnityEngine;

public class Maze : MonoBehaviour
{
    public int size = 10;
    public string nextScene = null;
    public float deathWallProbability = 0f;
    public float floorHoleProbability = 0f;

    public Material finishMaterial;
    public Material deathWallMaterial;

    void CreateMaze()
    {
        var gridGraph = GridGraphExtensions.Create(size);

        var mst = gridGraph.MST();

        int Index(int i, int j) => i * size + j;

        bool HasLeft(int i, int j)
        {
            if (j == 0)
            {
                return true;
            }

            var first = Index(i, j - 1);
            var second = Index(i, j);

            return mst.All(e => e.First != first || e.Second != second);
        }

        bool HasRight(int i, int j)
        {
            if (j == size - 1)
            {
                return true;
            }

            var first = Index(i, j);
            var second = Index(i, j + 1);

            return mst.All(e => e.First != first || e.Second != second);
        }

        bool HasTop(int i, int j)
        {
            if (i == 0)
            {
                return true;
            }

            var first = Index(i - 1, j);
            var second = Index(i, j);

            return mst.All(e => e.First != first || e.Second != second);
        }

        bool HasBottom(int i, int j)
        {
            if (i == size - 1)
            {
                return true;
            }

            var first = Index(i, j);
            var second = Index(i + 1, j);

            return mst.All(e => e.First != first || e.Second != second);
        }

        bool IsDeadEnd(int i, int j)
        {
            return
                Convert.ToInt32(HasLeft(i, j)) +
                Convert.ToInt32(HasRight(i, j)) +
                Convert.ToInt32(HasTop(i, j)) +
                Convert.ToInt32(HasBottom(i, j)) == 3;
        }

        var templateSingle = GameObject.Find("TemplateSingle");
        var templateDouble = GameObject.Find("TemplateDouble");
        var templateTriple = GameObject.Find("TemplateTriple");
        var templateCrossroads = GameObject.Find("TemplateCrossroads");
        var templateCorner = GameObject.Find("TemplateCorner");

        GameObject CreateTile(int i, int j)
        {
            var hasLeft = HasLeft(i, j);
            var hasRight = HasRight(i, j);
            var hasTop = HasTop(i, j);
            var hasBottom = HasBottom(i, j);

            var offset = transform.position + new Vector3(-j + size / 2f - 0.5f, 0, i - size / 2f + 0.5f);

            GameObject obj;

            if (hasLeft && hasRight && hasTop && hasBottom)
            {
                throw new NotImplementedException();
            }
            else if (hasLeft && hasRight && hasTop)
            {
                obj = Instantiate(templateTriple, transform);
                obj.transform.Translate(offset);
            }
            else if (hasLeft && hasRight && hasBottom)
            {
                obj = Instantiate(templateTriple, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 180, 0);
            }
            else if (hasLeft && hasTop && hasBottom)
            {
                obj = Instantiate(templateTriple, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 270, 0);
            }
            else if (hasRight && hasTop && hasBottom)
            {
                obj = Instantiate(templateTriple, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 90, 0);
            }
            else if (hasLeft && hasTop)
            {
                obj = Instantiate(templateCorner, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 270, 0);
            }
            else if (hasRight && hasTop)
            {
                obj = Instantiate(templateCorner, transform);
                obj.transform.Translate(offset);
            }
            else if (hasLeft && hasBottom)
            {
                obj = Instantiate(templateCorner, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 180, 0);
            }
            else if (hasRight && hasBottom)
            {
                obj = Instantiate(templateCorner, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 90, 0);
            }
            else if (hasLeft && hasRight)
            {
                obj = Instantiate(templateDouble, transform);
                obj.transform.Translate(offset);
            }
            else if (hasTop && hasBottom)
            {
                obj = Instantiate(templateDouble, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 90, 0);
            }
            else if (hasLeft)
            {
                obj = Instantiate(templateSingle, transform);
                obj.transform.Translate(offset);
            }
            else if (hasRight)
            {
                obj = Instantiate(templateSingle, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 180, 0);
            }
            else if (hasTop)
            {
                obj = Instantiate(templateSingle, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 90, 0);
            }
            else if (hasBottom)
            {
                obj = Instantiate(templateSingle, transform);
                obj.transform.Translate(offset);
                obj.transform.Rotate(0, 270, 0);
            }
            else
            {
                obj = Instantiate(templateCrossroads, transform);
                obj.transform.Translate(offset);
            }

            return obj;
        }

        bool IsStart(int i, int j)
        {
            return i + 1 == size && j + 1 == size;
        }

        bool IsEnd(int i, int j)
        {
            return i == 0 && j == 0;
        }

        var random = new System.Random();

        foreach (var i in Enumerable.Range(0, size))
        {
            foreach (var j in Enumerable.Range(0, size))
            {
                var tile = CreateTile(i, j);

                if (IsEnd(i, j))
                {
                    var floor = tile.transform.Find("Floor").gameObject;

                    floor.GetComponent<Renderer>().material = finishMaterial;
                    floor.AddComponent<Finish>();
                    floor.GetComponent<Finish>().nextScene = nextScene;
                }

                if (!IsEnd(i, j) && !IsStart(i, j))
                {
                    if (IsDeadEnd(i, j) && random.NextDouble() <= floorHoleProbability)
                    {
                        var floor = tile.transform.Find("Floor").gameObject;

                        Destroy(floor);
                    }
                    else if (random.NextDouble() <= deathWallProbability)
                    {
                        var wall = tile.transform.Find("Wall").gameObject;

                        wall.GetComponent<Renderer>().material = deathWallMaterial;
                        wall.AddComponent<DeathWall>();
                    }
                }
            }
        }

        Destroy(templateSingle);
        Destroy(templateDouble);
        Destroy(templateTriple);
        Destroy(templateCrossroads);
        Destroy(templateCorner);

        var templateBall = GameObject.Find("Ball");

        var ball = Instantiate(templateBall, transform);

        ball.transform.Translate(-size / 2f + 0.5f, 0, size / 2f - 0.5f);

        Destroy(templateBall);
    }

    void Start()
    {
        CreateMaze();
    }

    void Update()
    {
        
    }
}
