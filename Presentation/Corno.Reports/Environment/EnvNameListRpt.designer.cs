namespace Corno.Reports.Environment
{
    partial class EnvNameListRpt
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnvNameListRpt));
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.groupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.textBox32 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox33 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.table2 = new Telerik.Reporting.Table();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox37 = new Telerik.Reporting.TextBox();
            this.textBox38 = new Telerik.Reporting.TextBox();
            this.textBox39 = new Telerik.Reporting.TextBox();
            this.textBox47 = new Telerik.Reporting.TextBox();
            this.textBox52 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.sdsEnvList = new Telerik.Reporting.SqlDataSource();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox34 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.sdsCollege = new Telerik.Reporting.SqlDataSource();
            this.sdsCourse = new Telerik.Reporting.SqlDataSource();
            this.sdsCoursePart = new Telerik.Reporting.SqlDataSource();
            this.sdsBranch = new Telerik.Reporting.SqlDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.shape1 = new Telerik.Reporting.Shape();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // groupFooterSection
            // 
            this.groupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.052D);
            this.groupFooterSection.Name = "groupFooterSection";
            this.groupFooterSection.PageBreak = Telerik.Reporting.PageBreak.After;
            this.groupFooterSection.Style.Visible = false;
            // 
            // groupHeaderSection
            // 
            this.groupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(2.3D);
            this.groupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox32,
            this.textBox3,
            this.textBox33,
            this.textBox11,
            this.table2,
            this.textBox2,
            this.textBox8,
            this.textBox34,
            this.textBox13,
            this.textBox5,
            this.textBox6,
            this.textBox10,
            this.textBox19,
            this.textBox15,
            this.textBox21,
            this.textBox9});
            this.groupHeaderSection.Name = "groupHeaderSection";
            // 
            // textBox32
            // 
            this.textBox32.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox32.Name = "textBox32";
            this.textBox32.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox32.Style.Font.Bold = true;
            this.textBox32.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox32.StyleName = "";
            this.textBox32.Value = "= \"( \" + Fields.CourseID + \" ) \" + Fields.CourseName";
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox3.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox3.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.StyleName = "";
            this.textBox3.Value = "Course :";
            // 
            // textBox33
            // 
            this.textBox33.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.4D));
            this.textBox33.Name = "textBox33";
            this.textBox33.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox33.Style.Font.Bold = true;
            this.textBox33.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox33.StyleName = "";
            this.textBox33.Value = "= \"( \" + Fields.CoursePartID + \" ) \" + Fields.CoursePartName";
            // 
            // textBox11
            // 
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.4D));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox11.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox11.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox11.StyleName = "";
            this.textBox11.Value = "Course Part :";
            // 
            // table2
            // 
            this.table2.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "=ReportItem.DataObject"));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.504D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.187D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.624D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.308D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.16D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Cm(0.556D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Cm(0.489D)));
            this.table2.Body.SetCellContent(1, 0, this.textBox18);
            this.table2.Body.SetCellContent(1, 3, this.textBox1);
            this.table2.Body.SetCellContent(1, 4, this.textBox4);
            this.table2.Body.SetCellContent(0, 0, this.textBox37);
            this.table2.Body.SetCellContent(0, 3, this.textBox38);
            this.table2.Body.SetCellContent(0, 4, this.textBox39);
            this.table2.Body.SetCellContent(0, 2, this.textBox47);
            this.table2.Body.SetCellContent(1, 2, this.textBox52);
            this.table2.Body.SetCellContent(0, 1, this.textBox12);
            this.table2.Body.SetCellContent(1, 1, this.textBox14);
            tableGroup2.Name = "group";
            tableGroup3.Name = "Group6";
            this.table2.ColumnGroups.Add(tableGroup1);
            this.table2.ColumnGroups.Add(tableGroup2);
            this.table2.ColumnGroups.Add(tableGroup3);
            this.table2.ColumnGroups.Add(tableGroup4);
            this.table2.ColumnGroups.Add(tableGroup5);
            this.table2.ColumnHeadersPrintOnEveryPage = true;
            this.table2.DataSource = this.sdsEnvList;
            this.table2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox37,
            this.textBox12,
            this.textBox47,
            this.textBox38,
            this.textBox39,
            this.textBox18,
            this.textBox14,
            this.textBox52,
            this.textBox1,
            this.textBox4});
            this.table2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.1D));
            this.table2.Name = "table2";
            tableGroup6.Name = "Group2";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "DetailGroup";
            this.table2.RowGroups.Add(tableGroup6);
            this.table2.RowGroups.Add(tableGroup7);
            this.table2.RowHeadersPrintOnEveryPage = true;
            this.table2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.783D), Telerik.Reporting.Drawing.Unit.Cm(1.045D));
            this.table2.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.PRNNO", Telerik.Reporting.SortDirection.Asc));
            this.table2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.table2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            // 
            // textBox18
            // 
            this.textBox18.Format = "{0:N0}";
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.504D), Telerik.Reporting.Drawing.Unit.Cm(0.489D));
            this.textBox18.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox18.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox18.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox18.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox18.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox18.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox18.Value = "= RowNumber()";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.308D), Telerik.Reporting.Drawing.Unit.Cm(0.489D));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(4D);
            this.textBox1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "= Fields.StudentName";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.16D), Telerik.Reporting.Drawing.Unit.Cm(0.489D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox4.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox4.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox4.Value = "";
            // 
            // textBox37
            // 
            this.textBox37.Name = "textBox37";
            this.textBox37.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.504D), Telerik.Reporting.Drawing.Unit.Cm(0.556D));
            this.textBox37.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox37.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox37.Style.Font.Bold = true;
            this.textBox37.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox37.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox37.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox37.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox37.Value = "SR. No.";
            // 
            // textBox38
            // 
            this.textBox38.Name = "textBox38";
            this.textBox38.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.308D), Telerik.Reporting.Drawing.Unit.Cm(0.556D));
            this.textBox38.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox38.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox38.Style.Font.Bold = true;
            this.textBox38.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox38.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox38.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox38.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox38.Value = "STUDENT NAME";
            // 
            // textBox39
            // 
            this.textBox39.Name = "textBox39";
            this.textBox39.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.16D), Telerik.Reporting.Drawing.Unit.Cm(0.556D));
            this.textBox39.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox39.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox39.Style.Font.Bold = true;
            this.textBox39.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox39.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox39.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox39.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox39.Value = "Remarks";
            // 
            // textBox47
            // 
            this.textBox47.Name = "textBox47";
            this.textBox47.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.584D), Telerik.Reporting.Drawing.Unit.Cm(0.556D));
            this.textBox47.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox47.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox47.Style.Font.Bold = true;
            this.textBox47.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox47.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox47.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox47.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox47.StyleName = "";
            this.textBox47.Value = "Gender";
            // 
            // textBox52
            // 
            this.textBox52.Name = "textBox52";
            this.textBox52.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.584D), Telerik.Reporting.Drawing.Unit.Cm(0.489D));
            this.textBox52.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox52.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox52.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox52.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.textBox52.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox52.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox52.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox52.StyleName = "";
            this.textBox52.Value = "= Fields.Gender";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.187D), Telerik.Reporting.Drawing.Unit.Cm(0.556D));
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox12.StyleName = "";
            this.textBox12.Value = "PRN";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.187D), Telerik.Reporting.Drawing.Unit.Cm(0.489D));
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox14.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox14.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox14.StyleName = "";
            this.textBox14.Value = "=Fields.PRNNo";
            // 
            // sdsEnvList
            // 
            this.sdsEnvList.ConnectionString = "CoreContext (Corno.Bharati.OnlineExam)";
            this.sdsEnvList.Name = "sdsEnvList";
            this.sdsEnvList.Parameters.Add(new Telerik.Reporting.SqlDataSourceParameter("@InstanceID", System.Data.DbType.String, "41"));
            this.sdsEnvList.Parameters.Add(new Telerik.Reporting.SqlDataSourceParameter("@CourseID", System.Data.DbType.String, "=Parameters.Course.Value"));
            this.sdsEnvList.Parameters.Add(new Telerik.Reporting.SqlDataSourceParameter("@CollegeID", System.Data.DbType.String, "=Parameters.College.Value"));
            this.sdsEnvList.Parameters.Add(new Telerik.Reporting.SqlDataSourceParameter("@CoursePartID", System.Data.DbType.String, "=Parameters.CoursePart.Value"));
            this.sdsEnvList.SelectCommand = resources.GetString("sdsEnvList.SelectCommand");
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.6D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox2.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox2.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.StyleName = "";
            this.textBox2.Value = "Subject";
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.6D));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox8.Style.Font.Bold = true;
            this.textBox8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox8.StyleName = "";
            this.textBox8.Value = "ENVIRONMENTAL STUDIES";
            // 
            // textBox34
            // 
            this.textBox34.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox34.Name = "textBox34";
            this.textBox34.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox34.Style.Font.Bold = true;
            this.textBox34.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox34.StyleName = "";
            this.textBox34.Value = "= \"( \" + Fields.CollegeID+\") \" + Fields.CollegeName";
            // 
            // textBox13
            // 
            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox13.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox13.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox13.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox13.StyleName = "";
            this.textBox13.Value = "College :";
            // 
            // textBox5
            // 
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.8D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "= Corno.Reports.TelerikReportHelper.GetMonthName(CInt(Fields.Num_MONTH_NO)) + \" \"" +
    " + Fields.Num_YEAR";
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.8D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox6.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox6.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox6.StyleName = "";
            this.textBox6.Value = "Exam:";
            // 
            // textBox10
            // 
            this.textBox10.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.8D));
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox10.StyleName = "PageInfo";
            this.textBox10.Value = "Total Number of Students   :";
            // 
            // textBox19
            // 
            this.textBox19.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(2.084D));
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.9D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox19.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox19.Value = "Report Date:";
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3D), Telerik.Reporting.Drawing.Unit.Inch(2.084D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.3D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox15.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox15.Value = "= Fields.UserName";
            // 
            // textBox21
            // 
            this.textBox21.Format = "{0:dd/MM/yyyy hh:mm tt}";
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.9D), Telerik.Reporting.Drawing.Unit.Inch(2.084D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox21.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox21.Value = "= Now()";
            // 
            // textBox9
            // 
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.9D), Telerik.Reporting.Drawing.Unit.Inch(1.8D));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.6D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox9.StyleName = "PageInfo";
            this.textBox9.Value = "= \"(M) \"  + Count(IIF(Fields.Gender = \"M\", 1, Null))  + \" + (F) \"  + Count(IIF(Fi" +
    "elds.Gender = \"F\", 1, Null)) + \" = \"  + Count(Fields.Gender)";
            // 
            // sdsCollege
            // 
            this.sdsCollege.ConnectionString = "CoreContext (Corno.Bharati.OnlineExam)";
            this.sdsCollege.Name = "sdsCollege";
            this.sdsCollege.SelectCommand = "SELECT  Num_PK_COLLEGE_CD as ID,\r\n        Var_CL_COLLEGE_NM1 as Name\r\nFROM  Tbl_C" +
    "OLLEGE_MSTR\r\nWhere Chr_DELETE_FLG != \'Y\'";
            // 
            // sdsCourse
            // 
            this.sdsCourse.ConnectionString = "Corno.Reports.Properties.Settings.CoreContext";
            this.sdsCourse.Name = "sdsCourse";
            this.sdsCourse.Parameters.Add(new Telerik.Reporting.SqlDataSourceParameter("@NUM_FK_COLLEGE_CD", System.Data.DbType.String, "=Parameters.College.Value"));
            this.sdsCourse.SelectCommand = resources.GetString("sdsCourse.SelectCommand");
            // 
            // sdsCoursePart
            // 
            this.sdsCoursePart.ConnectionString = "CoreContext (Corno.Bharati.OnlineExam)";
            this.sdsCoursePart.Name = "sdsCoursePart";
            this.sdsCoursePart.Parameters.Add(new Telerik.Reporting.SqlDataSourceParameter("@CourseID", System.Data.DbType.String, "=Parameters.Course.Value"));
            this.sdsCoursePart.SelectCommand = "SELECT Num_PK_COPRT_NO as ID, Var_COPRT_DESC as Name, Num_FK_CO_CD as CourseID\r\n " +
    "     FROM Tbl_COURSE_PART_MSTR\r\nWhere Chr_DELETE_FLG != \'Y\' and Num_FK_CO_CD IN " +
    "(@CourseID)";
            // 
            // sdsBranch
            // 
            this.sdsBranch.ConnectionString = "CoreContext (Corno.Bharati.OnlineExam)";
            this.sdsBranch.Name = "sdsBranch";
            this.sdsBranch.Parameters.Add(new Telerik.Reporting.SqlDataSourceParameter("@NUM_PK_CO_CD", System.Data.DbType.String, "=Parameters.Course.Value"));
            this.sdsBranch.SelectCommand = resources.GetString("sdsBranch.SelectCommand");
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.6D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox20,
            this.textBox7,
            this.shape1});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // textBox20
            // 
            this.textBox20.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.8D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox20.Style.Font.Bold = true;
            this.textBox20.Style.Font.Name = "Arial";
            this.textBox20.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(15D);
            this.textBox20.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox20.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox20.Value = "Bharati Vidyapeeth Deemed University, Pune (India)";
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.8D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox7.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox7.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Name = "Arial";
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox7.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox7.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox7.Value = "Environmental Studies Student\'s List";
            // 
            // shape1
            // 
            this.shape1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.shape1.Name = "shape1";
            this.shape1.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.8D), Telerik.Reporting.Drawing.Unit.Inch(0.1D));
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.052D);
            this.detail.Name = "detail";
            this.detail.Style.Visible = false;
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.2D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox16,
            this.textBox17,
            this.textBox22});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // textBox16
            // 
            this.textBox16.Docking = Telerik.Reporting.DockingStyle.Right;
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.601D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox16.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox16.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox16.Value = "= PageNumber";
            // 
            // textBox17
            // 
            this.textBox17.Docking = Telerik.Reporting.DockingStyle.Left;
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.9D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox17.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox17.Value = "Report Date:";
            // 
            // textBox22
            // 
            this.textBox22.Docking = Telerik.Reporting.DockingStyle.Left;
            this.textBox22.Format = "{0:dd/MM/yyyy hh:mm tt}";
            this.textBox22.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.9D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox22.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox22.Value = "= Now()";
            // 
            // EnvNameListRpt
            // 
            this.DataSource = this.sdsEnvList;
            group1.DocumentMapText = "=Fields.CollegeName";
            group1.GroupFooter = this.groupFooterSection;
            group1.GroupHeader = this.groupHeaderSection;
            group1.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.CollegeID"));
            group1.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.CourseID"));
            group1.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.CoursePartID"));
            group1.GroupKeepTogether = Telerik.Reporting.GroupKeepTogether.All;
            group1.Name = "group1";
            group1.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.CollegeID", Telerik.Reporting.SortDirection.Asc));
            group1.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.CourseID", Telerik.Reporting.SortDirection.Asc));
            group1.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.CoursePartID", Telerik.Reporting.SortDirection.Asc));
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.groupHeaderSection,
            this.groupFooterSection,
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "CollegewiseStudent";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AvailableValues.DataSource = this.sdsCollege;
            reportParameter1.AvailableValues.DisplayMember = "= \"( \" + Fields.ID + \" ) \" + Fields.Name";
            reportParameter1.AvailableValues.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.ID", Telerik.Reporting.SortDirection.Asc));
            reportParameter1.AvailableValues.ValueMember = "= Fields.ID";
            reportParameter1.Mergeable = false;
            reportParameter1.MultiValue = true;
            reportParameter1.Name = "College";
            reportParameter1.Value = "";
            reportParameter1.Visible = true;
            reportParameter2.AvailableValues.DataSource = this.sdsCourse;
            reportParameter2.AvailableValues.DisplayMember = "= \"( \" + Fields.ID + \" ) \" + Fields.Name";
            reportParameter2.AvailableValues.Filters.Add(new Telerik.Reporting.Filter("=Fields.CollegeID", Telerik.Reporting.FilterOperator.In, "=Parameters.College.Value"));
            reportParameter2.AvailableValues.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.ID", Telerik.Reporting.SortDirection.Desc));
            reportParameter2.AvailableValues.ValueMember = "= Fields.ID";
            reportParameter2.MultiValue = true;
            reportParameter2.Name = "Course";
            reportParameter2.Value = "= Fields.ID";
            reportParameter2.Visible = true;
            reportParameter3.AvailableValues.DataSource = this.sdsCoursePart;
            reportParameter3.AvailableValues.DisplayMember = "= \"( \" + Fields.ID + \" ) \" + Fields.Name";
            reportParameter3.AvailableValues.Filters.Add(new Telerik.Reporting.Filter("=Fields.CourseID", Telerik.Reporting.FilterOperator.In, "=Parameters.Course.Value"));
            reportParameter3.AvailableValues.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.ID", Telerik.Reporting.SortDirection.Asc));
            reportParameter3.AvailableValues.ValueMember = "= Fields.ID";
            reportParameter3.MultiValue = true;
            reportParameter3.Name = "CoursePart";
            reportParameter3.Value = "= Fields.ID";
            reportParameter3.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.801D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox33;
        private Telerik.Reporting.Table table2;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox37;
        private Telerik.Reporting.TextBox textBox39;
        private Telerik.Reporting.TextBox textBox47;
        private Telerik.Reporting.TextBox textBox52;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.SqlDataSource sdsEnvList;
        private Telerik.Reporting.TextBox textBox32;
        private Telerik.Reporting.SqlDataSource sdsCollege;
        private Telerik.Reporting.SqlDataSource sdsCourse;
        private Telerik.Reporting.SqlDataSource sdsCoursePart;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection;
        private Telerik.Reporting.GroupFooterSection groupFooterSection;
        private Telerik.Reporting.Shape shape1;
        private Telerik.Reporting.SqlDataSource sdsBranch;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox34;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox38;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox22;
    }
}