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
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
namespace decode
{
    public partial class Form1 : Form
    {
        private String savepath = "";
        private String openpath = "";
        private String psw="nv86^E39%0_~!f3$^@#";
        private String columns = "R1|R2|HL|F2|R5|F9|F14|R80|F2|F9|F14|E2|E3|C1|C2|BH|CPMC|QYMC|CPMC|FMISPCNAME|E3|";
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
            
            
            
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                openpath = folderBrowserDialog1.SelectedPath;
                DirectoryInfo di = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                FileInfo[] files = di.GetFiles("*.dat");                       //扫描文件 GetFiles("*.txt");可以实现扫描扫描txt文件 
                foreach (FileInfo fi in files)
                {
                    try
                    {
                        DecryptFile(fi.FullName, Application.CommonAppDataPath +"temp.dat", psw);
                        listBox1.Items.Add(fi.FullName);
                    }
                    catch (Exception ex)
                    { }
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
        getInsertSQL();
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        string filesname = listBox1.SelectedItem.ToString();
        try
        {
            ds.Clear();
            ds.Tables.Clear();
            DecryptFile(filesname, getzipfilename(), psw);
              String xmlfilename=getxmlfilename();
              UnZipFile(getzipfilename(), xmlfilename);

              ds.ReadXmlSchema(xmlfilename );

              ds.ReadXml(xmlfilename, XmlReadMode.IgnoreSchema);
  
            ds = PlanData(ds, ds.Tables[0].TableName, columns);
            dataGridView1.DataSource = ds.Tables[0];
            File.Delete(xmlfilename);
            File.Delete(getzipfilename());
        }
        catch (Exception ex)
        {
            MessageBox.Show(filesname +"\t"+ ex.Message);
        }
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (comboBox1.SelectedIndex == 1)
        { comboBox2.Enabled = true; }
        else
        { comboBox2.Enabled = false; }
    }
    private void getInsertSQL()
    {
        if (savepath=="")  
        {
            if (this.folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                savepath = this.folderBrowserDialog1.SelectedPath;
            }
        }
   
        List<String> r=new List<String>();
        StreamWriter    file =  File.AppendText(savepath+ds.Tables[0].TableName+".sql");
        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
        {
            String rs = "insert into " + ds.Tables[0].TableName+" (";
        String column = "";
        String value = "";
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                column += "  " + ds.Tables[0].Columns[i].ColumnName;
                value += "'"+ds.Tables[0].Rows[j][i].ToString() +"'";
                if (i != ds.Tables[0].Columns.Count - 1)
                {
                    column += " ,";
                    value += " ,";
                }

            }

            rs += column + "  ) values (" + value + " )";
            file.WriteLine(rs);
            System.Windows.Forms.Application.DoEvents(); 
        }
        file.Flush(); 
        file.Close();
        
    }
    private string Reverse(string strReverse)
    {
        char[] array = strReverse.ToCharArray();
        Array.Reverse(array);
        return new string(array);
    }
    private string Decode(string strDecode)
    {
        string text = "";
        for (int i = 0; i < strDecode.Length / 4; i++)
        {
            text += (char)short.Parse(strDecode.Substring(i * 4, 4), NumberStyles.HexNumber);
        }
        return text;
    }
    public DataSet PlanData(DataSet dsSource, string strTableName, string strFields)
    {
        for (int i = 0; i < dsSource.Tables[strTableName].Rows.Count; i++)
        {
            for (int j = 0; j < dsSource.Tables[strTableName].Columns.Count; j++)
            {
                if (dsSource.Tables[strTableName].Rows[i][j].ToString().Trim() != "")
                {
                    if (strFields.IndexOf(dsSource.Tables[strTableName].Columns[j].ColumnName + "|") >= 0)
                    {
                        dsSource.Tables[strTableName].Rows[i][j] = this.Decode(this.Reverse(dsSource.Tables[strTableName].Rows[i][j].ToString().Trim().Substring(5)));
                    }
                }
            }
        }
        return dsSource;
    }

    private void button3_Click(object sender, System.EventArgs e)
    {
        ListViewItem item = new ListViewItem();
        item.Text = textBox1.Text;
        item.SubItems.Add(comboBox1.Text);
        item.SubItems.Add(comboBox2.Text);
            listView1.Items.Add(item);
    }
    }

}
