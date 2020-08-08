using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	public class PmxNode : IPmxObjectKey, IPmxStreamIO, ICloneable, INXName
	{
		public enum ElementType
		{
			Bone,
			Morph
		}

		public class NodeElement : IPmxObjectKey, IPmxStreamIO, ICloneable
		{
			public ElementType ElementType;

			public int Index;

			PmxObject IPmxObjectKey.ObjectKey
			{
				get
				{
					return PmxObject.NodeElement;
				}
			}

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
			{
				this.FromNodeElement(e);
			}

			public void FromNodeElement(NodeElement e)
			{
				this.ElementType = e.ElementType;
				this.Index = e.Index;
			}

			public void FromStreamEx(Stream s, PmxElementFormat f = null)
			{
				this.ElementType = (ElementType)s.ReadByte();
				switch (this.ElementType)
				{
				case ElementType.Bone:
					this.Index = PmxStreamHelper.ReadElement_Int32(s, f.BoneSize, true);
					break;
				case ElementType.Morph:
					this.Index = PmxStreamHelper.ReadElement_Int32(s, f.MorphSize, true);
					break;
				}
			}

			public void ToStreamEx(Stream s, PmxElementFormat f = null)
			{
				s.WriteByte((byte)this.ElementType);
				switch (this.ElementType)
				{
				case ElementType.Bone:
					PmxStreamHelper.WriteElement_Int32(s, this.Index, f.BoneSize, true);
					break;
				case ElementType.Morph:
					PmxStreamHelper.WriteElement_Int32(s, this.Index, f.MorphSize, true);
					break;
				}
			}

			public static NodeElement BoneElement(int index)
			{
				NodeElement nodeElement = new NodeElement();
				nodeElement.ElementType = ElementType.Bone;
				nodeElement.Index = index;
				return nodeElement;
			}

			public static NodeElement MorphElement(int index)
			{
				NodeElement nodeElement = new NodeElement();
				nodeElement.ElementType = ElementType.Morph;
				nodeElement.Index = index;
				return nodeElement;
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

		PmxObject IPmxObjectKey.ObjectKey
		{
			get
			{
				return PmxObject.Node;
			}
		}

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
				return this.Name;
			}
			set
			{
				this.Name = value;
			}
		}

		public PmxNode()
		{
			this.Name = "";
			this.NameE = "";
			this.SystemNode = false;
			this.ElementList = new List<NodeElement>();
		}

		public PmxNode(PmxNode node, bool nonStr = false)
			: this()
		{
			this.FromPmxNode(node, nonStr);
		}

		public void FromPmxNode(PmxNode node, bool nonStr = false)
		{
			if (!nonStr)
			{
				this.Name = node.Name;
				this.NameE = node.NameE;
			}
			this.SystemNode = node.SystemNode;
			int count = node.ElementList.Count;
			this.ElementList.Clear();
			this.ElementList.Capacity = count;
			for (int i = 0; i < count; i++)
			{
				this.ElementList.Add(node.ElementList[i].Clone());
			}
		}

		public void FromStreamEx(Stream s, PmxElementFormat f = null)
		{
			this.Name = PmxStreamHelper.ReadString(s, f);
			this.NameE = PmxStreamHelper.ReadString(s, f);
			this.SystemNode = (s.ReadByte() != 0);
			int num = PmxStreamHelper.ReadElement_Int32(s, 4, true);
			this.ElementList.Clear();
			this.ElementList.Capacity = num;
			for (int i = 0; i < num; i++)
			{
				NodeElement nodeElement = new NodeElement();
				nodeElement.FromStreamEx(s, f);
				this.ElementList.Add(nodeElement);
			}
		}

		public void ToStreamEx(Stream s, PmxElementFormat f = null)
		{
			PmxStreamHelper.WriteString(s, this.Name, f);
			PmxStreamHelper.WriteString(s, this.NameE, f);
			s.WriteByte((byte)(this.SystemNode ? 1 : 0));
			PmxStreamHelper.WriteElement_Int32(s, this.ElementList.Count, 4, true);
			for (int i = 0; i < this.ElementList.Count; i++)
			{
				this.ElementList[i].ToStreamEx(s, f);
			}
		}

		object ICloneable.Clone()
		{
			return new PmxNode(this, false);
		}

		public PmxNode Clone()
		{
			return new PmxNode(this, false);
		}
	}
}
