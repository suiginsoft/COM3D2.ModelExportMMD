using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	internal class Vmd : IBytesConvert, ICloneable
	{
		internal enum VmdVersion
		{
			v2,
			v1
		}

		internal enum NormalizeDataType
		{
			All,
			Motion,
			Skin,
			Camera,
			Light,
			SelfShadow,
			VisibleIK
		}

		public string VMDHeader = "Vocaloid Motion Data 0002";

		private const int HeaderBytes = 30;

		private const string HeaderString_V1 = "Vocaloid Motion Data file";

		private const string HeaderString_V2 = "Vocaloid Motion Data 0002";

		public const string CameraHeaderName = "カメラ・照明";

		public string ModelName = "";

		public int ModelNameBytes = 20;

		private const int ModelNameBytes_V1 = 10;

		private const int ModelNameBytes_V2 = 20;

		private VmdVersion m_ver;

		public List<VmdMotion> MotionList = new List<VmdMotion>();

		public List<VmdMorph> MorphList = new List<VmdMorph>();

		public List<VmdCamera> CameraList = new List<VmdCamera>();

		public List<VmdLight> LightList = new List<VmdLight>();

		public List<VmdSelfShadow> SelfShadowList = new List<VmdSelfShadow>();

		public List<VmdVisibleIK> VisibleIKList = new List<VmdVisibleIK>();

		public VmdVersion Version => m_ver;

		public int ByteCount => HeaderBytes + ModelNameBytes + GetListBytes(MotionList) + GetListBytes(MorphList) + GetListBytes(VisibleIKList) + GetListBytes(CameraList) + GetListBytes(LightList) + GetListBytes(SelfShadowList);

		public Vmd()
		{
		}

		public Vmd(Vmd vmd)
			: this()
		{
			VMDHeader = vmd.VMDHeader;
			ModelName = vmd.ModelName;
			MotionList = CP.CloneList(vmd.MotionList);
			MorphList = CP.CloneList(vmd.MorphList);
			CameraList = CP.CloneList(vmd.CameraList);
			LightList = CP.CloneList(vmd.LightList);
			SelfShadowList = CP.CloneList(vmd.SelfShadowList);
			VisibleIKList = CP.CloneList(vmd.VisibleIKList);
		}

		public Vmd(string path)
			: this()
		{
			FromFile(path);
		}

		public void FromFile(string path)
		{
			byte[] bytes = File.ReadAllBytes(path);
			FromBytes(bytes, 0);
		}

		public void NormalizeList(NormalizeDataType type)
		{
			switch (type)
			{
			case NormalizeDataType.Motion:
				MotionList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.Skin:
				MorphList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.Camera:
				CameraList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.Light:
				LightList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.SelfShadow:
				SelfShadowList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.VisibleIK:
				VisibleIKList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.All:
				MotionList.Sort(VmdFrameBase.Compare);
				MorphList.Sort(VmdFrameBase.Compare);
				CameraList.Sort(VmdFrameBase.Compare);
				LightList.Sort(VmdFrameBase.Compare);
				SelfShadowList.Sort(VmdFrameBase.Compare);
				VisibleIKList.Sort(VmdFrameBase.Compare);
				break;
			}
		}

		public byte[] ToBytes()
		{
			NormalizeList(NormalizeDataType.All);
			List<byte> list = new List<byte>();
			byte[] array = new byte[30];
			BytesStringProc.SetString(array, VMDHeader, 0, 0);
			list.AddRange(array);
			byte[] array2 = new byte[ModelNameBytes];
			BytesStringProc.SetString(array2, ModelName, 0, 253);
			list.AddRange(array2);
			int count = MotionList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int i = 0; i < count; i++)
			{
				list.AddRange(MotionList[i].ToBytes());
			}
			count = MorphList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int j = 0; j < count; j++)
			{
				list.AddRange(MorphList[j].ToBytes());
			}
			count = VisibleIKList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int k = 0; k < count; k++)
			{
				list.AddRange(VisibleIKList[k].ToBytes());
			}
			count = CameraList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int l = 0; l < count; l++)
			{
				list.AddRange(CameraList[l].ToBytes());
			}
			count = LightList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int m = 0; m < count; m++)
			{
				list.AddRange(LightList[m].ToBytes());
			}
			count = SelfShadowList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int n = 0; n < count; n++)
			{
				list.AddRange(SelfShadowList[n].ToBytes());
			}
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			int num = startIndex;
			byte[] array = new byte[30];
			Array.Copy(bytes, num, array, 0, 30);
			VMDHeader = BytesStringProc.GetString(array, 0);
			int num2 = 0;
			if (string.Compare(VMDHeader, "Vocaloid Motion Data file", ignoreCase: true) == 0)
			{
				ModelNameBytes = 10;
				num2 = 1;
				m_ver = VmdVersion.v1;
			}
			else
			{
				if (string.Compare(VMDHeader, "Vocaloid Motion Data 0002", ignoreCase: true) != 0)
				{
					throw new Exception("対応したVMDファイルではありません");
				}
				ModelNameBytes = 20;
				num2 = 2;
				m_ver = VmdVersion.v2;
			}
			num += 30;
			_ = new byte[ModelNameBytes];
			Array.Copy(bytes, num, array, 0, ModelNameBytes);
			ModelName = BytesStringProc.GetString(array, 0);
			num += ModelNameBytes;
			int num3 = BitConverter.ToInt32(bytes, num);
			num += 4;
			MotionList.Clear();
			MotionList.Capacity = num3;
			for (int i = 0; i < num3; i++)
			{
				VmdMotion vmdMotion = new VmdMotion();
				vmdMotion.FromBytes(bytes, num);
				num += vmdMotion.ByteCount;
				MotionList.Add(vmdMotion);
			}
			if (bytes.Length <= num)
			{
				return;
			}
			num3 = BitConverter.ToInt32(bytes, num);
			num += 4;
			MorphList.Clear();
			MorphList.Capacity = num3;
			for (int j = 0; j < num3; j++)
			{
				VmdMorph vmdMorph = new VmdMorph();
				vmdMorph.FromBytes(bytes, num);
				num += vmdMorph.ByteCount;
				MorphList.Add(vmdMorph);
			}
			if (bytes.Length <= num)
			{
				return;
			}
			num3 = BitConverter.ToInt32(bytes, num);
			num += 4;
			CameraList.Clear();
			CameraList.Capacity = num3;
			switch (num2)
			{
			case 1:
			{
				for (int l = 0; l < num3; l++)
				{
					VmdCamera_v1 vmdCamera_v = new VmdCamera_v1();
					vmdCamera_v.FromBytes(bytes, num);
					num += vmdCamera_v.ByteCount;
					CameraList.Add(vmdCamera_v.ToVmdCamera());
				}
				break;
			}
			case 2:
			{
				for (int k = 0; k < num3; k++)
				{
					VmdCamera vmdCamera = new VmdCamera();
					vmdCamera.FromBytes(bytes, num);
					num += vmdCamera.ByteCount;
					CameraList.Add(vmdCamera);
				}
				break;
			}
			}
			if (bytes.Length <= num)
			{
				return;
			}
			num3 = BitConverter.ToInt32(bytes, num);
			num += 4;
			LightList.Clear();
			LightList.Capacity = num3;
			for (int m = 0; m < num3; m++)
			{
				VmdLight vmdLight = new VmdLight();
				vmdLight.FromBytes(bytes, num);
				num += vmdLight.ByteCount;
				LightList.Add(vmdLight);
			}
			if (bytes.Length <= num)
			{
				return;
			}
			num3 = BitConverter.ToInt32(bytes, num);
			num += 4;
			SelfShadowList.Clear();
			SelfShadowList.Capacity = num3;
			for (int n = 0; n < num3; n++)
			{
				VmdSelfShadow vmdSelfShadow = new VmdSelfShadow();
				vmdSelfShadow.FromBytes(bytes, num);
				num += vmdSelfShadow.ByteCount;
				SelfShadowList.Add(vmdSelfShadow);
			}
			if (bytes.Length > num)
			{
				num3 = BitConverter.ToInt32(bytes, num);
				num += 4;
				VisibleIKList.Clear();
				VisibleIKList.Capacity = num3;
				for (int num4 = 0; num4 < num3; num4++)
				{
					VmdVisibleIK vmdVisibleIK = new VmdVisibleIK();
					vmdVisibleIK.FromBytes(bytes, num);
					num += vmdVisibleIK.ByteCount;
					VisibleIKList.Add(vmdVisibleIK);
				}
			}
		}

		public object Clone()
		{
			return new Vmd(this);
		}

		public static int GetListBytes<T>(List<T> list) where T : IBytesConvert
		{
			int count = list.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				num += list[i].ByteCount;
			}
			return num;
		}
	}
}
