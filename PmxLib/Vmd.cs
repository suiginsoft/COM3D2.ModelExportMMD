using System;
using System.Collections.Generic;
using System.IO;

namespace PmxLib
{
	public class Vmd : IBytesConvert, ICloneable
	{
		public enum VmdVersion
		{
			v2,
			v1
		}

		public enum NormalizeDataType
		{
			All,
			Motion,
			Skin,
			Camera,
			Light,
			SelfShadow,
			VisibleIK
		}

		private const int HeaderBytes = 30;

		private const string HeaderString_V1 = "Vocaloid Motion Data file";

		private const string HeaderString_V2 = "Vocaloid Motion Data 0002";

		public const string CameraHeaderName = "カメラ・照明";

		private const int ModelNameBytes_V1 = 10;

		private const int ModelNameBytes_V2 = 20;

		public string VMDHeader = "Vocaloid Motion Data 0002";

		public string ModelName = "";

		public int ModelNameBytes = 20;

		private VmdVersion m_ver;

		public List<VmdMotion> MotionList = new List<VmdMotion>();

		public List<VmdMorph> MorphList = new List<VmdMorph>();

		public List<VmdCamera> CameraList = new List<VmdCamera>();

		public List<VmdLight> LightList = new List<VmdLight>();

		public List<VmdSelfShadow> SelfShadowList = new List<VmdSelfShadow>();

		public List<VmdVisibleIK> VisibleIKList = new List<VmdVisibleIK>();

		public VmdVersion Version
		{
			get
			{
				return this.m_ver;
			}
		}

		public int ByteCount
		{
			get
			{
				return 30 + this.ModelNameBytes + Vmd.GetListBytes(this.MotionList) + Vmd.GetListBytes(this.MorphList) + Vmd.GetListBytes(this.VisibleIKList) + Vmd.GetListBytes(this.CameraList) + Vmd.GetListBytes(this.LightList) + Vmd.GetListBytes(this.SelfShadowList);
			}
		}

		public Vmd()
		{
			PmxLibClass.IsLocked();
		}

		public Vmd(Vmd vmd)
			: this()
		{
			this.VMDHeader = vmd.VMDHeader;
			this.ModelName = vmd.ModelName;
			this.MotionList = CP.CloneList(vmd.MotionList);
			this.MorphList = CP.CloneList(vmd.MorphList);
			this.CameraList = CP.CloneList(vmd.CameraList);
			this.LightList = CP.CloneList(vmd.LightList);
			this.SelfShadowList = CP.CloneList(vmd.SelfShadowList);
			this.VisibleIKList = CP.CloneList(vmd.VisibleIKList);
		}

		public Vmd(string path)
			: this()
		{
			this.FromFile(path);
		}

		public void FromFile(string path)
		{
			try
			{
				byte[] bytes = File.ReadAllBytes(path);
				this.FromBytes(bytes, 0);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void NormalizeList(NormalizeDataType type)
		{
			switch (type)
			{
			case NormalizeDataType.All:
				this.MotionList.Sort(VmdFrameBase.Compare);
				this.MorphList.Sort(VmdFrameBase.Compare);
				this.CameraList.Sort(VmdFrameBase.Compare);
				this.LightList.Sort(VmdFrameBase.Compare);
				this.SelfShadowList.Sort(VmdFrameBase.Compare);
				this.VisibleIKList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.Motion:
				this.MotionList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.Skin:
				this.MorphList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.Camera:
				this.CameraList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.Light:
				this.LightList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.SelfShadow:
				this.SelfShadowList.Sort(VmdFrameBase.Compare);
				break;
			case NormalizeDataType.VisibleIK:
				this.VisibleIKList.Sort(VmdFrameBase.Compare);
				break;
			}
		}

		public byte[] ToBytes()
		{
			this.NormalizeList(NormalizeDataType.All);
			List<byte> list = new List<byte>();
			byte[] array = new byte[30];
			BytesStringProc.SetString(array, this.VMDHeader, 0, 0);
			list.AddRange(array);
			byte[] array2 = new byte[this.ModelNameBytes];
			BytesStringProc.SetString(array2, this.ModelName, 0, 253);
			list.AddRange(array2);
			int count = this.MotionList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int i = 0; i < count; i++)
			{
				list.AddRange(this.MotionList[i].ToBytes());
			}
			count = this.MorphList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int j = 0; j < count; j++)
			{
				list.AddRange(this.MorphList[j].ToBytes());
			}
			count = this.VisibleIKList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int k = 0; k < count; k++)
			{
				list.AddRange(this.VisibleIKList[k].ToBytes());
			}
			count = this.CameraList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int l = 0; l < count; l++)
			{
				list.AddRange(this.CameraList[l].ToBytes());
			}
			count = this.LightList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int m = 0; m < count; m++)
			{
				list.AddRange(this.LightList[m].ToBytes());
			}
			count = this.SelfShadowList.Count;
			list.AddRange(BitConverter.GetBytes(count));
			for (int n = 0; n < count; n++)
			{
				list.AddRange(this.SelfShadowList[n].ToBytes());
			}
			return list.ToArray();
		}

		public void FromBytes(byte[] bytes, int startIndex)
		{
			byte[] array = new byte[30];
			Array.Copy(bytes, startIndex, array, 0, 30);
			this.VMDHeader = BytesStringProc.GetString(array, 0);
			int num;
			if (string.Compare(this.VMDHeader, "Vocaloid Motion Data file", true) == 0)
			{
				this.ModelNameBytes = 10;
				num = 1;
				this.m_ver = VmdVersion.v1;
			}
			else
			{
				if (string.Compare(this.VMDHeader, "Vocaloid Motion Data 0002", true) != 0)
				{
					throw new Exception("対応したVMDファイルではありません");
				}
				this.ModelNameBytes = 20;
				num = 2;
				this.m_ver = VmdVersion.v2;
			}
			int num2 = startIndex + 30;
			Array.Copy(bytes, num2, array, 0, this.ModelNameBytes);
			this.ModelName = BytesStringProc.GetString(array, 0);
			num2 += this.ModelNameBytes;
			int num3 = BitConverter.ToInt32(bytes, num2);
			num2 += 4;
			this.MotionList.Clear();
			this.MotionList.Capacity = num3;
			for (int i = 0; i < num3; i++)
			{
				VmdMotion vmdMotion = new VmdMotion();
				vmdMotion.FromBytes(bytes, num2);
				num2 += vmdMotion.ByteCount;
				this.MotionList.Add(vmdMotion);
			}
			if (bytes.Length > num2)
			{
				num3 = BitConverter.ToInt32(bytes, num2);
				num2 += 4;
				this.MorphList.Clear();
				this.MorphList.Capacity = num3;
				for (int j = 0; j < num3; j++)
				{
					VmdMorph vmdMorph = new VmdMorph();
					vmdMorph.FromBytes(bytes, num2);
					num2 += vmdMorph.ByteCount;
					this.MorphList.Add(vmdMorph);
				}
				if (bytes.Length > num2)
				{
					num3 = BitConverter.ToInt32(bytes, num2);
					num2 += 4;
					this.CameraList.Clear();
					this.CameraList.Capacity = num3;
					switch (num)
					{
					case 1:
						for (int l = 0; l < num3; l++)
						{
							VmdCamera_v1 vmdCamera_v = new VmdCamera_v1();
							vmdCamera_v.FromBytes(bytes, num2);
							num2 += vmdCamera_v.ByteCount;
							this.CameraList.Add(vmdCamera_v.ToVmdCamera());
						}
						break;
					case 2:
						for (int k = 0; k < num3; k++)
						{
							VmdCamera vmdCamera = new VmdCamera();
							vmdCamera.FromBytes(bytes, num2);
							num2 += vmdCamera.ByteCount;
							this.CameraList.Add(vmdCamera);
						}
						break;
					}
					if (bytes.Length > num2)
					{
						num3 = BitConverter.ToInt32(bytes, num2);
						num2 += 4;
						this.LightList.Clear();
						this.LightList.Capacity = num3;
						for (int m = 0; m < num3; m++)
						{
							VmdLight vmdLight = new VmdLight();
							vmdLight.FromBytes(bytes, num2);
							num2 += vmdLight.ByteCount;
							this.LightList.Add(vmdLight);
						}
						if (bytes.Length > num2)
						{
							num3 = BitConverter.ToInt32(bytes, num2);
							num2 += 4;
							this.SelfShadowList.Clear();
							this.SelfShadowList.Capacity = num3;
							for (int n = 0; n < num3; n++)
							{
								VmdSelfShadow vmdSelfShadow = new VmdSelfShadow();
								vmdSelfShadow.FromBytes(bytes, num2);
								num2 += vmdSelfShadow.ByteCount;
								this.SelfShadowList.Add(vmdSelfShadow);
							}
							if (bytes.Length > num2)
							{
								num3 = BitConverter.ToInt32(bytes, num2);
								num2 += 4;
								this.VisibleIKList.Clear();
								this.VisibleIKList.Capacity = num3;
								for (int num4 = 0; num4 < num3; num4++)
								{
									VmdVisibleIK vmdVisibleIK = new VmdVisibleIK();
									vmdVisibleIK.FromBytes(bytes, num2);
									num2 += vmdVisibleIK.ByteCount;
									this.VisibleIKList.Add(vmdVisibleIK);
								}
							}
						}
					}
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
				int num2 = num;
				num = num2 + list[i].ByteCount;
			}
			return num;
		}
	}
}
