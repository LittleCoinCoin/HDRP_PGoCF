using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth.DataModel;


class PlantViewPortAnnotationDef : AnnotationDefinition
{
	public PlantViewPortAnnotationDef(string id)
		: base(id) { }

	public override string modelType => "Plant Viewport Annotation Def";
	public override string description => "The position of the target plant in the camera's local space as a proportion of width (x) and height (y)";
}

struct PlantViewPortAnnotationData
{
	public uint instance_id;
	public Vector2 ViewPort;
}

[Serializable]
class PlantViewPortAnnotation : Annotation
{
	public PlantViewPortAnnotationData[] plantViewPortAnnotations;
	public PlantViewPortAnnotation(AnnotationDefinition _definition, string _sensorId, PlantViewPortAnnotationData[] _plantViewPortAnnotations)
		: base(_definition, _sensorId)
	{
		plantViewPortAnnotations = _plantViewPortAnnotations;
	}

	public override void ToMessage(IMessageBuilder builder)
	{
		base.ToMessage(builder);

		for (int i = 0; i < plantViewPortAnnotations.Length; i++)
		{
			IMessageBuilder values = builder.AddNestedMessageToVector("values");
			values.AddUInt("Plant instance ID", plantViewPortAnnotations[i].instance_id);
			values.AddFloatArray("Plant position", MessageBuilderUtils.ToFloatVector(plantViewPortAnnotations[i].ViewPort));
		}
	}

	public override bool IsValid() => true;

}
