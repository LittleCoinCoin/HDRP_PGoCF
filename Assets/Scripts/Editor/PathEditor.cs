using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Working on the path in the scene view
// A path is defined by 
// - point A and point B
// - Number of intermediary points
// - Tangents orientation

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    PathCreator creator;
    Path path;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        Vector3 initial_point_inspector = EditorGUILayout.Vector3Field("Initial point", creator.init_point);
        if (initial_point_inspector != path.initial_point)
        {
            creator.init_point = initial_point_inspector;
            path.initial_point = initial_point_inspector;
            path.MovePoint(0, initial_point_inspector);
        }

        Vector3 end_point_inspector = EditorGUILayout.Vector3Field("Final point", creator.end_point);
        if (end_point_inspector != path.final_point)
        {
            creator.end_point = end_point_inspector;
            path.final_point = end_point_inspector;
            path.MovePoint(path.NumPoints - 1, end_point_inspector);
        }

        int nb_points_in_path = EditorGUILayout.IntField("Number of points", creator.nb_points);
        if (nb_points_in_path != path.nb_points)
        {
            creator.NbPoints = nb_points_in_path;
            creator.CreatePath();
            path = creator.path;
        }

        bool auto_set_control_points = GUILayout.Toggle(path.AutoSetControlPoints, "Auto Set Control Points");
        if (auto_set_control_points != path.AutoSetControlPoints)
        {
            Undo.RecordObject(creator, "Toggle auto set control points");
            path.AutoSetControlPoints = auto_set_control_points;
        }

        if (GUILayout.Button("Reset path"))
        {
            creator.CreatePath();
            path = creator.path;
        }

/*        
 *        int nb_rows = EditorGUILayout.IntField("Number of rows", creator.nb_rows);
        if (nb_rows != creator.nb_rows)
        {
            creator.nb_rows = nb_rows;
        }

        float inter_row_distance = EditorGUILayout.FloatField("Inter row distance", creator.inter_row_distance);
        if (inter_row_distance != creator.inter_row_distance)
        {
            creator.inter_row_distance = inter_row_distance;
        }
        
        if (GUILayout.Button("Generate all rows"))
        {
            creator.nb_rows = nb_rows;
            creator.inter_row_distance = inter_row_distance;
            creator.GenerateAllRows();
        }

        if (EditorGUI.EndChangeCheck()) {
            SceneView.RepaintAll();
        }*/

    }

    void OnSceneGUI()
    {
        /*Input();*/
        Draw();
    }

// For now we disable the initialization of new points using mouse interactions
// to focus on initialization of a given number of points

/*    void Input()
    {
        Event gui_event = Event.current;
        Vector3 mouse_position = HandleUtility.GUIPointToWorldRay(
                                                    gui_event.mousePosition).origin;
        if ((gui_event.type == EventType.MouseDown && gui_event.button == 0 && gui_event.shift)
            || (gui_event.isMouse && gui_event.type == EventType.MouseDown && gui_event.clickCount == 5))
        {
            Undo.RecordObject(creator, "Add Segment");
            path.AddSegment(mouse_position);
        }
    }*/

    void Draw()
    {
        for (int i = 0; i < path.NumSegments; i++)
        {
            Vector3[] points = path.GetPointsInSegment(i);
            Handles.DrawBezier(points[0], points[3], points[1], points[2],
                                Color.blue, null, 7);
            Handles.color = Color.black;
            Handles.DrawLine(points[0], points[1]); // anchor to tangent
            Handles.DrawLine(points[2], points[3]);
        }

        Handles.color = new Color(1f, 0.33f, 0.23f, 1f);
        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 new_pos = Handles.FreeMoveHandle(path[i], Quaternion.identity, 
                              0.7f, Vector3.zero, Handles.SphereHandleCap);
            if (path[i] != new_pos)
            {
                Undo.RecordObject(creator, "Move Point");
                path.MovePoint(i, new_pos);

                // update inspector values
                if (i == 0)
                {
                    // Round method for Vector3 is implemented in ExtensionMethods
                    creator.init_point = new_pos.Round(2);
                    path.initial_point = new_pos.Round(2);
                }

                if (i == path.NumPoints - 1)
                {
                    creator.end_point = new_pos.Round(2);
                    path.final_point = new_pos.Round(2);
                }

            }
        }
    }

    private void OnEnable()
    {
        creator = (PathCreator)target;
        if (creator.path == null)
        {
            creator.CreatePath();
        }

        path = creator.path;
    }
}
