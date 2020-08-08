using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	internal class PmxNode : PmxIDObject, IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		public enum ElementType
		{
			Bone,
			Morph
		}

		public class NodeElement : PmxIDObject, IPmxObjectKey, IPmxStreamIO, ICloneable
		{
			public ElementType ElementType;

			public int Index;

			PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.NodeElement;

			public PmxBone RefBone
			{
				get;
				set;
			}

			public PmxMorph RefMorph
			{
				get;
				set;
			}

			public NodeElement()
			{
			}

			public NodeElement(NodeElement e)
				: this()
			{
				FromNodeElement(e);
			}

			public void FromNodeElement(NodeElement e)
			{
				ElementType = e.ElementType;
				Index = e.Index;
				FromID(e);
			}

			public void FromStreamEx(Stream s, PmxElementFormat f = null)
			{
				ElementType = (ElementType)s.ReadByte();
				switch (ElementType)
				{
				case ElementType.Bone:
					Index = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize);
					break;
				case ElementType.Morph:
					Index = PmxStreamHelper.ReadElement_Int32(s, f.MorphSize);
					break;
				}
				if (f.WithID)
				{
					base.UID = PmxStreamHelper.ReadElement_UInt(s);
					base.CID = PmxStreamHelper.ReadElement_UInt(s);
				}
			}

			public void ToStreamEx(Stream s, PmxElementFormat f = null)
			{
				s.WriteByte((byte)ElementType);
				switch (ElementType)
				{
				case ElementType.Bone:
					PmxStreamHelper.WriteElement_Int32(s, Index, f.BoneSize);
					break;
				case ElementType.Morph:
					PmxStreamHelper.WriteElement_Int32(s, Index, f.MorphSize);
					break;
				}
				if (f.WithID)
				{
					PmxStreamHelper.WriteElement_UInt(s, base.UID);
					PmxStreamHelper.WriteElement_UInt(s, base.CID);
				}
			}

			public static NodeElement BoneElement(int index)
			{
				return new NodeElement
				{
					ElementType = ElementType.Bone,
					Index = index
				};
			}

			public static NodeElement MorphElement(int index)
			{
				return new NodeElement
				{
					ElementType = ElementType.Morph,
					Index = index
				};
			}

			object ICloneable.Clone()
			{
				return new NodeElement(this);
			}

			public NodeElement Clone()
			{
				return new NodeElement(this);
			}
		}

		public bool SystemNode;

		public List<NodeElement> ElementList;

		PmxObjectType IPmxObjectKey.ObjectKey => PmxObjectType.Node;

		public string Name
		{
			get;
			set;
		}

		public string NameE
		{
			get;
			set;
		}

		public string NXName
		{
			get
			{
				return Name;
			}
			set
			{
				Name = value;
			}
		}

		public PmxNode()
		{
			Name = "";
			NameE = "";
			SystemNode = false;
			ElementList = new List<NodeElement>();
		}

		public PmxNode(PmxNode node, bool nonStr = false)
			: this()
		{
			FromPmxNode(node, nonStr);
		}

		public void FromPmxNode(PmxNode node, bool nonStr = false)
		{
			if (!nonStr)
			{
				Name = node.Name;
				NameE = node.NameE;
			}
			SystemNode = node.SystemNode;
			int count = node.ElementList.Count;
			ElementList.Clear();
			ElementList.Capacity = count;
			for (int i = 0; i < count; i++)
			{
				ElementList.Add(node.ElementList[i].Clone());
			}
			FromID(node);
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			Name = PmxStreamHelper.ReadString(s, f);
			NameE = PmxStreamHelper.ReadString(s, f);
			SystemNode = (s.ReadByte() != 0);
			int num = PmxStreamHelper.ReadElement_Int32(s);
			ElementList.Clear();
			ElementList.Capacity = num;
			for (int i = 0; i < num; i++)
			{
				NodeElement nodeElement = new NodeElement();
				nodeElement.FromStreamEx(s, f);
				ElementList.Add(nodeElement);
			}
			if (f.WithID)
			{
				base.UID = PmxStreamHelper.ReadElement_UInt(s);
				base.CID = PmxStreamHelper.ReadElement_UInt(s);
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, Name, f);
			PmxStreamHelper.WriteString(s, NameE, f);
			s.WriteByte((byte)(SystemNode ? 1u : 0u));
			PmxStreamHelper.WriteElement_Int32(s, ElementList.Count);
			for (int i = 0; i < ElementList.Count; i++)
			{
				ElementList[i].ToStreamEx(s, f);
			}
			if (f.WithID)
			{
				PmxStreamHelper.WriteElement_UInt(s, base.UID);
				PmxStreamHelper.WriteElement_UInt(s, base.CID);
			}
		}

		object ICloneable.Clone()
		{
			return new PmxNode(this);
		}

		public PmxNode Clone()
		{
			return new PmxNode(this);
		}
	}
}
