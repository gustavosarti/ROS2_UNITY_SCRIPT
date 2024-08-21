using UnityEngine;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;

public class PointCloudSubscriber : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/camera/depth/color/points";
    public MeshFilter meshFilter;
    public Material pointMaterial;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] indices;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PointCloud2Msg>(topicName, PointCloudCallback);

        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    void PointCloudCallback(PointCloud2Msg msg)
    {
        // Limpar a malha atual
        vertices = new Vector3[msg.data.Length / (int)msg.point_step];
        indices = new int[vertices.Length];

        // Converter os dados do PointCloud2Msg para exibir no Unity
        int pointStep = (int)msg.point_step;
        for (int i = 0; i < vertices.Length; i++)
        {
            float x = System.BitConverter.ToSingle(msg.data, i * pointStep + 0);
            float y = System.BitConverter.ToSingle(msg.data, i * pointStep + 4);
            float z = System.BitConverter.ToSingle(msg.data, i * pointStep + 8);

            vertices[i] = new Vector3(x, y, z);
            indices[i] = i;
        }

        // Atualizar a malha
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        mesh.RecalculateBounds();
    }
}
