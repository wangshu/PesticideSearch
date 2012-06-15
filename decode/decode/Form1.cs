using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;
namespace decode
{
    public partial class Form1 : Form
    {
    
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.Enabled = false; 
        }
        private DataSet ds = new DataSet();
        private String getxmlfilename()
        {
            return Directory.GetCurrentDirectory() + "temp.xml";
        }
        private String getzipfilename()
        {
            return Directory.GetCurrentDirectory() + "temp.zip";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {

                    listBox1.Items.Add(openFileDialog1.FileNames[i]);

                    
                }
            }
        }
   
    public static void DecryptFile(string inFile, string outFile, string password)
		{
			using (FileStream fileStream = File.OpenRead(inFile))
			{
				using (FileStream fileStream2 = File.OpenWrite(outFile))
				{
					int num = (int)fileStream.Length;
					byte[] array = new byte[131072];
					int num2 = 0;
					byte[] array2 = new byte[16];
					fileStream.Read(array2, 0, 16);
					byte[] array3 = new byte[16];
					fileStream.Read(array3, 0, 16);
					SymmetricAlgorithm symmetricAlgorithm = CreateRijndael(password, array3);
					symmetricAlgorithm.IV = array2;
					int num3 = 32;
					long num4 = -1L;
					HashAlgorithm hashAlgorithm = SHA256.Create();
					using (CryptoStream cryptoStream = new CryptoStream(fileStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Read))
					{
						using (CryptoStream cryptoStream2 = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write))
						{
							BinaryReader binaryReader = new BinaryReader(cryptoStream);
							num4 = binaryReader.ReadInt64();
							ulong num5 = binaryReader.ReadUInt64();
							if (18158797384510146255uL != num5)
							{
								throw new Exception("文件被破坏");
							}
							long num6 = num4 / 131072L;
							long num7 = num4 % 131072L;
							int num8 = 0;
							int num9;
							while ((long)num8 < num6)
							{
								num9 = cryptoStream.Read(array, 0, array.Length);
								fileStream2.Write(array, 0, num9);
								cryptoStream2.Write(array, 0, num9);
								num3 += num9;
								num2 += num9;
								num8++;
							}
							if (num7 > 0L)
							{
								num9 = cryptoStream.Read(array, 0, (int)num7);
								fileStream2.Write(array, 0, num9);
								cryptoStream2.Write(array, 0, num9);
								num3 += num9;
								num2 += num9;
							}
							cryptoStream2.Flush();
							cryptoStream2.Close();
							fileStream2.Flush();
							fileStream2.Close();
							byte[] hash = hashAlgorithm.Hash;
							byte[] array4 = new byte[hashAlgorithm.HashSize / 8];
							num9 = cryptoStream.Read(array4, 0, array4.Length);
							if (array4.Length != num9 || !CheckByteArrays(array4, hash))
							{
								throw new  Exception("文件被破坏");
							}
						}
					}
					if ((long)num2 != num4)
					{
						throw new Exception("文件大小不匹配");
					}
				}
			}
		}
    private static SymmetricAlgorithm CreateRijndael(string password, byte[] salt)
    {
        PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, salt, "SHA256", 1000);
        SymmetricAlgorithm symmetricAlgorithm = Rijndael.Create();
        symmetricAlgorithm.KeySize = 256;
        symmetricAlgorithm.Key = passwordDeriveBytes.GetBytes(32);
        symmetricAlgorithm.Padding = PaddingMode.PKCS7;
        return symmetricAlgorithm;
    }
    private static bool CheckByteArrays(byte[] b1, byte[] b2)
    {
        bool result;
        if (b1.Length == b2.Length)
        {
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    result = false;
                    return result;
                }
            }
            result = true;
        }
        else
        {
            result = false;
        }
        return result;
    }
    public void UnZipFile(string ZipedFile, string UnZipFile)
    {
        ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(ZipedFile));
        ZipEntry nextEntry;
        while ((nextEntry = zipInputStream.GetNextEntry()) != null)
        {
            string directoryName = Path.GetDirectoryName(UnZipFile);
            string fileName = Path.GetFileName(nextEntry.Name);
            Directory.CreateDirectory(directoryName);
            if (fileName != string.Empty)
            {
                FileStream fileStream = File.OpenWrite(UnZipFile);
                byte[] array = new byte[2048];
                while (true)
                {
                    int num = zipInputStream.Read(array, 0, array.Length);
                    if (num <= 0)
                    {
                        break;
                    }
                    fileStream.Write(array, 0, num);
                }
                fileStream.Close();
            }
        }
        zipInputStream.Close();
    }

    private void button2_Click(object sender, EventArgs e)
    {

    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        string filesname = listBox1.SelectedItem.ToString();
        try
        {
            ds.Clear();
            DecryptFile(filesname, getzipfilename(), "nv86^E39%0_~!f3$^@#");
            UnZipFile(getzipfilename(), getxmlfilename());
            ds.ReadXmlSchema(getxmlfilename());
            comboBox2.Items.Clear();
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                comboBox2.Items.Add(ds.Tables[0].Columns[i].Caption);
            }
            dataGridView1.DataSource = ds.Tables[0];
            
        }
        catch (Exception ex)
        {
            MessageBox.Show(filesname);
        }
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (comboBox1.SelectedIndex == 1)
        { comboBox2.Enabled = true; }
        else
        { comboBox2.Enabled = false; }
    }
    }

}
