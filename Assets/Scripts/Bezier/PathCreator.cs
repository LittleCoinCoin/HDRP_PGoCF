using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creating the path and holding a reference to it

public class PathCreator : MonoBehaviour
{
    [HideInInspector]
    public Path path;

    [HideInInspector]
    public Vector3 init_point = new Vector3(4, 0, 4);
    [HideInInspector]
    public Vector3 end_point = new Vector3(-4, 0, -4);

    [HideInInspector]
    public int nb_rows;

    [HideInInspector]
    public float inter_row_distance;

    // field containing the rows
    // used to constrain movements of points
    public GameObject field;

    // number of points at path initialization
    [Range(0, 10), HideInInspector]
    public int nb_points = 3;

    public int NbPoints
    {
        get
        {
            return nb_points;
        }
        set
        {
            nb_points = value;
        }
    }

    // initialize a path
    public void CreatePath()
    {
        if (nb_points > 2)
        {
            path = new Path(init_point, end_point, field, nb_points);
        }
        else
        {
            path = new Path(init_point, end_point, field);
        }
    }

    // initialize a path from an existing one
    public void CreatePath(Path ref_path)
    {
        nb_points = ref_path.nb_points;
        init_point = ref_path.initial_point;
        end_point = ref_path.final_point;
        path = new Path(ref_path.Points, ref_path.field);
    }

    // To translate a path from a given list of points
    public void TranslatePath(Vector3 translation)
    {
        Path translated_path = new Path(path.Points, field);

        // translating the copy
        for (int i = 0; i < path.Points.Count; i += 3)
        {
            translated_path.MovePoint(i, path.Points[i] + translation);
        }

        init_point += translation;
        end_point += translation;
        path = translated_path;
    }
}
