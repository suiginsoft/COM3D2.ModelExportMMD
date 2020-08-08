using UnityEngine;
using System;
using System.Xml;
using System.Text;

/**
 * Collada Exporter
 *
 * @author      Michael Grenier
 * @author_url  http://mgrenier.me
 * @copyright   2011 (c) Michael Grenier
 * @license     MIT - http://www.opensource.org/licenses/MIT
 *
 * @example
 *
 *              ColladaExporter export = new ColladaExporter("path/to/export.dae", replace_or_not);
 *              export.AddGeometry("MyMeshId", mesh_object);
 *              export.AddGeometryToScene("MyMeshId", "MyMeshName");
 *              export.Save();
 *
 */

public class ColladaExporter : IDisposable
{
    protected string path;
    public const string COLLADA = "http://www.collada.org/2005/11/COLLADASchema";

    public XmlDocument xml
    {
        get;
        protected set;
    }

    public XmlNamespaceManager nsManager
    {
        get;
        protected set;
    }

    protected XmlNode root;
    protected XmlNode cameras;
    protected XmlNode lights;
    protected XmlNode images;
    protected XmlNode effects;
    protected XmlNode materials;
    protected XmlNode geometries;
    protected XmlNode animations;
    protected XmlNode controllers;
    protected XmlNode visual_scenes;
    protected XmlNode default_scene;
    protected XmlNode scene;

    public ColladaExporter(String path)
    : this(path, true)
    {
    }

    public ColladaExporter(String path, bool replace)
    {
        this.path = path;
        this.xml = new XmlDocument();

        this.nsManager = new XmlNamespaceManager(this.xml.NameTable);
        this.nsManager.AddNamespace("x", COLLADA);

        if (!replace)
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(path);
                this.xml.Load(reader);
                reader.Close();
                reader = null;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        else
            this.xml.AppendChild(this.xml.CreateXmlDeclaration("1.0", "UTF-8", null));

        XmlAttribute attr;

        this.root = this.xml.SelectSingleNode("/x:COLLADA", this.nsManager);
        if (this.root == null)
        {
            this.root = this.xml.AppendChild(this.xml.CreateElement("COLLADA", COLLADA));
            attr = this.xml.CreateAttribute("version");
            attr.Value = "1.4.1";
            this.root.Attributes.Append(attr);
        }

        XmlNode node;

        // Create asset
        {
            node = this.root.SelectSingleNode("/x:asset", this.nsManager);
            if (node == null)
            {
                this.root
                .AppendChild(
                    this.xml.CreateElement("asset", COLLADA)
                    .AppendChild(
                        this.xml.CreateElement("contributor", COLLADA)
                        .AppendChild(
                            this.xml.CreateElement("author", COLLADA)
                            .AppendChild(this.xml.CreateTextNode("Unity3D User"))
                            .ParentNode
                        )
                        .ParentNode
                        .AppendChild(
                            this.xml.CreateElement("author_tool", COLLADA)
                            .AppendChild(this.xml.CreateTextNode("Unity " + Application.unityVersion))
                            .ParentNode
                        )
                        .ParentNode
                    )
                    .ParentNode
                    .AppendChild(
                        this.xml.CreateElement("created", COLLADA)
                        .AppendChild(this.xml.CreateTextNode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00")))
                        .ParentNode
                    )
                    .ParentNode
                    .AppendChild(
                        this.xml.CreateElement("modified", COLLADA)
                        .AppendChild(this.xml.CreateTextNode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00")))
                        .ParentNode
                    )
                    .ParentNode
                    .AppendChild(
                        this.xml.CreateElement("up_axis", COLLADA)
                        .AppendChild(this.xml.CreateTextNode("Y_UP"))
                        .ParentNode
                    )
                    .ParentNode
                );
            }
        }

        // Create libraries
        this.cameras = this.root.SelectSingleNode("/x:library_cameras", this.nsManager);
        if (this.cameras == null)
            this.cameras = this.root.AppendChild(this.xml.CreateElement("library_cameras", COLLADA));
        this.lights = this.root.SelectSingleNode("/x:library_lights", this.nsManager);
        if (this.lights == null)
            this.lights = this.root.AppendChild(this.xml.CreateElement("library_lights", COLLADA));
        this.images = this.root.SelectSingleNode("/x:library_images", this.nsManager);
        if (this.images == null)
            this.images = this.root.AppendChild(this.xml.CreateElement("library_images", COLLADA));
        this.effects = this.root.SelectSingleNode("/x:library_effects", this.nsManager);
        if (this.effects == null)
            this.effects = this.root.AppendChild(this.xml.CreateElement("library_effects", COLLADA));
        this.materials = this.root.SelectSingleNode("/x:library_materials", this.nsManager);
        if (this.materials == null)
            this.materials = this.root.AppendChild(this.xml.CreateElement("library_materials", COLLADA));
        this.geometries = this.root.SelectSingleNode("/x:library_geometries", this.nsManager);
        if (this.geometries == null)
            this.geometries = this.root.AppendChild(this.xml.CreateElement("library_geometries", COLLADA));
        this.animations = this.root.SelectSingleNode("/x:library_animations", this.nsManager);
        if (this.animations == null)
            this.animations = this.root.AppendChild(this.xml.CreateElement("library_animations", COLLADA));
        this.controllers = this.root.SelectSingleNode("/x:library_controllers", this.nsManager);
        if (this.controllers == null)
            this.controllers = this.root.AppendChild(this.xml.CreateElement("library_controllers", COLLADA));
        this.visual_scenes = this.root.SelectSingleNode("/x:library_visual_scenes", this.nsManager);
        if (this.visual_scenes == null)
        {
            this.visual_scenes = this.root.AppendChild(this.xml.CreateElement("library_visual_scenes", COLLADA));
            this.default_scene = this.visual_scenes.AppendChild(this.xml.CreateElement("visual_scene", COLLADA));
            attr = this.xml.CreateAttribute("id");
            attr.Value = "Scene";
            this.default_scene.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("name");
            attr.Value = "Scene";
            this.default_scene.Attributes.Append(attr);
        }
        this.scene = this.root.SelectSingleNode("/x:library_scene", this.nsManager);
        if (this.scene == null)
        {
            this.scene = this.root.AppendChild(this.xml.CreateElement("scene", COLLADA));
            node = this.scene.AppendChild(this.xml.CreateElement("instance_visual_scene", COLLADA));
            attr = this.xml.CreateAttribute("url");
            attr.Value = "#Scene";
            node.Attributes.Append(attr);
        }
    }

    public void Dispose()
    {
    }

    public void Save()
    {
        this.xml.Save(this.path);
    }

    public XmlNode AddGeometry(string id, Mesh sourceMesh, Transform transform)
    {
        XmlNode geometry = this.geometries.AppendChild(this.xml.CreateElement("geometry", COLLADA));
        XmlNode mesh = geometry.AppendChild(this.xml.CreateElement("mesh", COLLADA));
        XmlNode nodeA, nodeB, nodeC, nodeD;
        XmlAttribute attr;
        StringBuilder str;

        attr = this.xml.CreateAttribute("id");
        attr.Value = id + "-mesh";
        geometry.Attributes.Append(attr);

        // Positions
        if (sourceMesh.vertexCount > 0)
        {
            nodeA = mesh.AppendChild(this.xml.CreateElement("source", COLLADA));
            attr = this.xml.CreateAttribute("id");
            attr.Value = id + "-mesh-positions";
            nodeA.Attributes.Append(attr);

            nodeB = nodeA.AppendChild(this.xml.CreateElement("float_array", COLLADA));
            attr = this.xml.CreateAttribute("id");
            attr.Value = id + "-mesh-positions-array";
            nodeB.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("count");
            attr.Value = (sourceMesh.vertexCount * 3).ToString();
            nodeB.Attributes.Append(attr);

            str = new StringBuilder();
            for (int i = 0, n = sourceMesh.vertexCount; i < n; ++i)
            {
                // Scene positioning
                Vector3 v = sourceMesh.vertices[i];
                if (transform != null)
                {
                    v.Scale(transform.lossyScale);
                    v = transform.rotation * v;
                    v = transform.position + v;
                }
                v.x = -v.x;
                str.Append(v.x.ToString());
                str.Append(" ");
                str.Append(v.y.ToString());
                str.Append(" ");
                str.Append(v.z.ToString());
                if (i + 1 != n)
                    str.Append(" ");
            }
            nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
            str = null;

            nodeB = nodeA.AppendChild(this.xml.CreateElement("technique_common", COLLADA));
            nodeC = nodeB.AppendChild(this.xml.CreateElement("accessor", COLLADA));
            attr = this.xml.CreateAttribute("source");
            attr.Value = "#" + id + "-mesh-positions-array";
            nodeC.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("count");
            attr.Value = sourceMesh.vertexCount.ToString();
            nodeC.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("stride");
            attr.Value = "3";
            nodeC.Attributes.Append(attr);
            nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
            attr = this.xml.CreateAttribute("name");
            attr.Value = "X";
            nodeD.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("type");
            attr.Value = "float";
            nodeD.Attributes.Append(attr);
            nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
            attr = this.xml.CreateAttribute("name");
            attr.Value = "Y";
            nodeD.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("type");
            attr.Value = "float";
            nodeD.Attributes.Append(attr);
            nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
            attr = this.xml.CreateAttribute("name");
            attr.Value = "Z";
            nodeD.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("type");
            attr.Value = "float";
            nodeD.Attributes.Append(attr);
        }

        // Colors
        if (sourceMesh.colors.Length > 0)
        {
            nodeA = mesh.AppendChild(this.xml.CreateElement("source", COLLADA));
            attr = this.xml.CreateAttribute("id");
            attr.Value = id + "-mesh-colors";
            nodeA.Attributes.Append(attr);

            nodeB = nodeA.AppendChild(this.xml.CreateElement("float_array", COLLADA));
            attr = this.xml.CreateAttribute("id");
            attr.Value = id + "-mesh-colors-array";
            nodeB.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("count");
            attr.Value = (sourceMesh.colors.Length * 3).ToString();
            nodeB.Attributes.Append(attr);

            str = new StringBuilder();
            for (int i = 0, n = sourceMesh.colors.Length; i < n; ++i)
            {
                //str.Append(mesh.colors[i].a.ToString());
                //str.Append(" ");
                str.Append(sourceMesh.colors[i].r.ToString());
                str.Append(" ");
                str.Append(sourceMesh.colors[i].g.ToString());
                str.Append(" ");
                str.Append(sourceMesh.colors[i].b.ToString());
                if (i + 1 != n)
                    str.Append(" ");
            }
            nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
            str = null;

            nodeB = nodeA.AppendChild(this.xml.CreateElement("technique_common", COLLADA));
            nodeC = nodeB.AppendChild(this.xml.CreateElement("accessor", COLLADA));
            attr = this.xml.CreateAttribute("source");
            attr.Value = "#" + id + "-mesh-colors-array";
            nodeC.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("count");
            attr.Value = sourceMesh.colors.Length.ToString();
            nodeC.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("stride");
            attr.Value = "3";
            nodeC.Attributes.Append(attr);
            nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
            attr = this.xml.CreateAttribute("name");
            attr.Value = "R";
            nodeD.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("type");
            attr.Value = "float";
            nodeD.Attributes.Append(attr);
            nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
            attr = this.xml.CreateAttribute("name");
            attr.Value = "G";
            nodeD.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("type");
            attr.Value = "float";
            nodeD.Attributes.Append(attr);
            nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
            attr = this.xml.CreateAttribute("name");
            attr.Value = "B";
            nodeD.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("type");
            attr.Value = "float";
            nodeD.Attributes.Append(attr);
        }

        // Normals
        if (sourceMesh.normals.Length > 0)
        {
            nodeA = mesh.AppendChild(this.xml.CreateElement("source", COLLADA));
            attr = this.xml.CreateAttribute("id");
            attr.Value = id + "-mesh-normals";
            nodeA.Attributes.Append(attr);

            nodeB = nodeA.AppendChild(this.xml.CreateElement("float_array", COLLADA));
            attr = this.xml.CreateAttribute("id");
            attr.Value = id + "-mesh-normals-array";
            nodeB.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("count");
            attr.Value = (sourceMesh.normals.Length * 3).ToString();
            nodeB.Attributes.Append(attr);

            str = new StringBuilder();
            for (int i = 0, n = sourceMesh.normals.Length; i < n; ++i)
            {
                str.Append((-sourceMesh.normals[i].x).ToString());
                str.Append(" ");
                str.Append(sourceMesh.normals[i].y.ToString());
                str.Append(" ");
                str.Append(sourceMesh.normals[i].z.ToString());
                if (i + 1 != n)
                    str.Append(" ");
            }
            nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
            str = null;

            nodeB = nodeA.AppendChild(this.xml.CreateElement("technique_common", COLLADA));
            nodeC = nodeB.AppendChild(this.xml.CreateElement("accessor", COLLADA));
            attr = this.xml.CreateAttribute("source");
            attr.Value = "#" + id + "-mesh-normals-array";
            nodeC.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("count");
            attr.Value = sourceMesh.normals.Length.ToString();
            nodeC.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("stride");
            attr.Value = "3";
            nodeC.Attributes.Append(attr);
            nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
            attr = this.xml.CreateAttribute("name");
            attr.Value = "X";
            nodeD.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("type");
            attr.Value = "float";
            nodeD.Attributes.Append(attr);
            nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
            attr = this.xml.CreateAttribute("name");
            attr.Value = "Y";
            nodeD.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("type");
            attr.Value = "float";
            nodeD.Attributes.Append(attr);
            nodeD = nodeC.AppendChild(this.xml.CreateElement("param", COLLADA));
            attr = this.xml.CreateAttribute("name");
            attr.Value = "Z";
            nodeD.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("type");
            attr.Value = "float";
            nodeD.Attributes.Append(attr);
        }

        // Vertices
        {
            nodeA = mesh.AppendChild(this.xml.CreateElement("vertices", COLLADA));
            attr = this.xml.CreateAttribute("id");
            attr.Value = id + "-mesh-vertices";
            nodeA.Attributes.Append(attr);

            if (sourceMesh.vertexCount > 0)
            {
                nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
                attr = this.xml.CreateAttribute("semantic");
                attr.Value = "POSITION";
                nodeB.Attributes.Append(attr);
                attr = this.xml.CreateAttribute("source");
                attr.Value = "#" + id + "-mesh-positions";
                nodeB.Attributes.Append(attr);
            }

            if (sourceMesh.normals.Length > 0)
            {
                nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
                attr = this.xml.CreateAttribute("semantic");
                attr.Value = "NORMAL";
                nodeB.Attributes.Append(attr);
                attr = this.xml.CreateAttribute("source");
                attr.Value = "#" + id + "-mesh-normals";
                nodeB.Attributes.Append(attr);
            }

            if (sourceMesh.colors.Length > 0)
            {
                nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
                attr = this.xml.CreateAttribute("semantic");
                attr.Value = "COLOR";
                nodeB.Attributes.Append(attr);
                attr = this.xml.CreateAttribute("source");
                attr.Value = "#" + id + "-mesh-colors";
                nodeB.Attributes.Append(attr);
            }
        }

        // Triangles
        {
            nodeA = mesh.AppendChild(this.xml.CreateElement("triangles", COLLADA));
            attr = this.xml.CreateAttribute("count");
            attr.Value = (sourceMesh.triangles.Length / 3).ToString();
            nodeA.Attributes.Append(attr);

            nodeB = nodeA.AppendChild(this.xml.CreateElement("input", COLLADA));
            attr = this.xml.CreateAttribute("semantic");
            attr.Value = "VERTEX";
            nodeB.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("source");
            attr.Value = "#" + id + "-mesh-vertices";
            nodeB.Attributes.Append(attr);
            attr = this.xml.CreateAttribute("offset");
            attr.Value = "0";
            nodeB.Attributes.Append(attr);

            nodeB = nodeA.AppendChild(this.xml.CreateElement("p", COLLADA));

            str = new StringBuilder();
            for (int i = 0; i < sourceMesh.triangles.Length / 3; ++i)
            {
                int p0 = sourceMesh.triangles[3 * i + 0];
                int p1 = sourceMesh.triangles[3 * i + 1];
                int p2 = sourceMesh.triangles[3 * i + 2];
                // Collada uses reverse winding order from unity
                str.Append(p2 + " ");
                str.Append(p1 + " ");
                str.Append(p0 + " ");
            }

            nodeB.AppendChild(this.xml.CreateTextNode(str.ToString()));
            str = null;
        }

        return geometry;
    }

    public XmlNode AddGeometryToScene(string id, string name)
    {
        return AddGeometryToScene(id, name, Matrix4x4.identity, null);
    }

    public XmlNode AddGeometryToScene(string id, string name, Matrix4x4 matrix)
    {
        return AddGeometryToScene(id, name, matrix, null);
    }

    public XmlNode AddGeometryToScene(string id, string name, Matrix4x4 matrix, XmlNode parent)
    {
        XmlNode nodeA, nodeB;
        XmlAttribute attr;

        if (parent == null)
            parent = this.default_scene;

        nodeA = parent.AppendChild(this.xml.CreateElement("node", COLLADA));
        attr = this.xml.CreateAttribute("id");
        attr.Value = id;
        nodeA.Attributes.Append(attr);
        attr = this.xml.CreateAttribute("type");
        attr.Value = "Node";
        nodeA.Attributes.Append(attr);
        attr = this.xml.CreateAttribute("name");
        attr.Value = name;
        nodeA.Attributes.Append(attr);

        nodeB = nodeA.AppendChild(this.xml.CreateElement("matrix", COLLADA));
        attr = this.xml.CreateAttribute("sid");
        attr.Value = "matrix";
        nodeB.Attributes.Append(attr);
        nodeB.AppendChild(this.xml.CreateTextNode(matrix.ToString()));

        nodeB = nodeA.AppendChild(this.xml.CreateElement("instance_geometry", COLLADA));
        attr = this.xml.CreateAttribute("url");
        attr.Value = "#" + id + "-mesh";
        nodeB.Attributes.Append(attr);

        return nodeA;
    }

    public XmlNode AddEmptyToScene(string id, string name)
    {
        return AddEmptyToScene(id, name, Matrix4x4.identity, null);
    }

    public XmlNode AddEmptyToScene(string id, string name, Matrix4x4 matrix)
    {
        return AddEmptyToScene(id, name, matrix, null);
    }

    public XmlNode AddEmptyToScene(string id, string name, Matrix4x4 matrix, XmlNode parent)
    {
        XmlNode nodeA, nodeB;
        XmlAttribute attr;

        if (parent == null)
            parent = this.default_scene;

        nodeA = parent.AppendChild(this.xml.CreateElement("node", COLLADA));
        attr = this.xml.CreateAttribute("id");
        attr.Value = id;
        nodeA.Attributes.Append(attr);
        attr = this.xml.CreateAttribute("type");
        attr.Value = "Node";
        nodeA.Attributes.Append(attr);
        attr = this.xml.CreateAttribute("name");
        attr.Value = name;
        nodeA.Attributes.Append(attr);

        nodeB = nodeA.AppendChild(this.xml.CreateElement("matrix", COLLADA));
        attr = this.xml.CreateAttribute("sid");
        attr.Value = "matrix";
        nodeB.Attributes.Append(attr);
        nodeB.AppendChild(this.xml.CreateTextNode(matrix.ToString()));

        return nodeA;
    }
}