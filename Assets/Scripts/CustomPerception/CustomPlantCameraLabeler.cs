using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.DataModel;
using UnityEngine.Rendering;

public class CustomPlantCameraLabeler : CameraLabeler
{
	public override string description => "Labeler for Plants";
	public override string labelerId => "Labeler for Plants";
	protected override bool supportsVisualization => false;

	private AnnotationDefinition plantAnnotationDefinition;

	public Camera camera;
	public Field fieldGeneratorRef;
	private float[] cameraBoundaries;

	protected override void Setup()
	{
		plantAnnotationDefinition = new PlantViewPortAnnotationDef("C0B4A22C-0420-4D9F-BAFC-954B8F7B35A7");
		DatasetCapture.RegisterAnnotationDefinition(plantAnnotationDefinition);
	}

	protected override void OnBeginRendering(ScriptableRenderContext scriptableRenderContext)
	{
		if (sensorHandle.ShouldCaptureThisFrame)
		{
			cameraBoundaries = camera.GetComponent<CameraVision>().VisionBoundariesOnField();

			List<PlantViewPortAnnotationData> plantViewPortAnnotations = new List<PlantViewPortAnnotationData>();

			for (int i = 0; i < fieldGeneratorRef.all_target_plants.Count; i++)
			{
				GameObject _targetPlant = fieldGeneratorRef.all_target_plants[i];
				if (_targetPlant.activeSelf &&
					(_targetPlant.transform.position.x > cameraBoundaries[3] && _targetPlant.transform.position.x < cameraBoundaries[2]) &&
					(_targetPlant.transform.position.z > cameraBoundaries[1] && _targetPlant.transform.position.z < cameraBoundaries[0]))
				{
					Vector3 _correctedTransform = new Vector3(_targetPlant.transform.position.x,
																_targetPlant.transform.position.y + _targetPlant.transform.localScale.y * _targetPlant.GetComponent<MeshFilter>().sharedMesh.bounds.size.y,//_targetPlant.transform.localScale.y,
																_targetPlant.transform.position.z);

					Vector2 _corrected_plant_ViewPort = camera.WorldToViewportPoint(_correctedTransform);

					if (0 < _corrected_plant_ViewPort.x && _corrected_plant_ViewPort.x < 1 &&
							0 < _corrected_plant_ViewPort.y && _corrected_plant_ViewPort.y < 1)
					{
						plantViewPortAnnotations.Add(new PlantViewPortAnnotationData
						{
							instance_id = _targetPlant.GetComponent<UnityEngine.Perception.GroundTruth.LabelManagement.Labeling>().instanceId,
							ViewPort = new Vector2(_corrected_plant_ViewPort.x, _corrected_plant_ViewPort.y)
						});
					}
				}
			}
			PlantViewPortAnnotation pvpAnnotation = new PlantViewPortAnnotation(plantAnnotationDefinition, sensorHandle.Id, plantViewPortAnnotations.ToArray());
			sensorHandle.ReportAnnotation(plantAnnotationDefinition, pvpAnnotation);
		}
	}
}