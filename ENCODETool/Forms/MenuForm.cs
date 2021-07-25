using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

using System.Drawing;

namespace ENCODE.Base
{
    public partial class MenuForm : Form
    {
        private SplitContainer general_spl;
        private TrackBar merge_class_trb;
        private ComboBox profile_cmb;
        private CheckBox read_write_chk;
        private CheckBox blit_chk;
        private CheckBox order_chk;
        private CheckBox merge_system_chk;
        private CheckBox merge_class_chk;
        private CheckBox lock_chk;
        private TrackBar merge_system_trb;
        private Label label1;
        private TrackBar perf_maint_trb;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private GroupBox prof_param_grp;
        private ComboBox project_cmb;
        private Label application_lbl;
        private Button search_btn;
        private Button prep_btn;
        private ProgressBar total_prg;
        private ProgressBar step_prg;
        private Button plan_btn;
        private Label step_prg_lbl;
        private Button start_btn;
        private Label total_prg_lbl;
        private GroupBox input_grp;
        private SplitContainer parameters_spl;
        private SplitContainer result_spl;
        private TabControl result_tab_con;
        private TabPage compare_tab;
        private TabPage view_tab;
        private ComboBox compare_left_item_cmb;
        private ComboBox compare_left_type_cmb;
        private ComboBox compare_right_item_cmb;
        private ComboBox compare_right_type_cmb;
        private bool changingPlanProfile;
        private TabPage advice_tab;
        private GroupBox specific_ad_grp;
        private GroupBox gen_ad_grp;
        private Label introduction_ad_lbl;
        private Label general_ad_lbl;
        private Label specific_ad_lbl;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer3;
        private ComboBox view_type_cmb;
        private ComboBox view_item_cmb;
        private Panel compare_left_pnl;
        private Panel compare_right_pnl;
        private Button save_view_btn;
        private SplitContainer splitContainer4;
        private SplitContainer splitContainer2;

        private Panel view_pnl;
        private TableLayoutPanel result_tbl_view;
        private Label label7;

        public MenuForm()
        {
            InitializeComponent();
            InitValues();
        }

        private void InitValues()
        {
            changingPlanProfile = false;

            // Prepare profile dropdown
            var cmbProfileObjects = new object[Program.planProfile.AvailablePlanProfiles.Count + 1];
            for (int i = 0; i < Program.planProfile.AvailablePlanProfiles.Count; i++)
            {
                cmbProfileObjects[i] = Program.planProfile.AvailablePlanProfiles[i].ProfileName;
            }
            cmbProfileObjects[cmbProfileObjects.Length - 1] = "Custom";
            this.profile_cmb.Items.AddRange(cmbProfileObjects);

            // Select the current profile
            Program.planProfile.ChangePerformanceOrientation(perf_maint_trb.Value);
            profile_cmb.SelectedItem = Program.planProfile.ProfileName;
            this.perf_maint_trb.Maximum = Program.planProfile.PerformanceSteps - 1;
            lock_chk.Checked = true;

            // Prepare project options
            var cmbProjectObjects = new object[] {
                @"C:\"
            };
            this.project_cmb.Items.AddRange(cmbProjectObjects);
            this.project_cmb.SelectedIndex = 0;

            // Add general advice
            string generalAdvice = string.Empty;
            foreach (string advice in Constant.GENERAL_RULES)
            {
                generalAdvice += $"- {advice} \n \n";
            }
            this.general_ad_lbl.Text = generalAdvice;


            // Prepare Compare Left dropdowns
            this.compare_left_type_cmb.Items.AddRange(new object[] { "Class", "Method" });

            // Prepare Compare rigt dropdowns
            this.compare_right_type_cmb.Items.AddRange(new object[] { "Entity", "System" });

            // Prepare View dropdowns
            this.view_type_cmb.Items.AddRange(new object[] { "Class", "Method", "Entity", "Component", "System" });

        }

        private void InitializeComponent()
        {
            this.general_spl = new System.Windows.Forms.SplitContainer();
            this.parameters_spl = new System.Windows.Forms.SplitContainer();
            this.prof_param_grp = new System.Windows.Forms.GroupBox();
            this.merge_class_chk = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.read_write_chk = new System.Windows.Forms.CheckBox();
            this.profile_cmb = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lock_chk = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.blit_chk = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.merge_class_trb = new System.Windows.Forms.TrackBar();
            this.order_chk = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.merge_system_chk = new System.Windows.Forms.CheckBox();
            this.perf_maint_trb = new System.Windows.Forms.TrackBar();
            this.merge_system_trb = new System.Windows.Forms.TrackBar();
            this.input_grp = new System.Windows.Forms.GroupBox();
            this.project_cmb = new System.Windows.Forms.ComboBox();
            this.application_lbl = new System.Windows.Forms.Label();
            this.plan_btn = new System.Windows.Forms.Button();
            this.search_btn = new System.Windows.Forms.Button();
            this.step_prg_lbl = new System.Windows.Forms.Label();
            this.prep_btn = new System.Windows.Forms.Button();
            this.step_prg = new System.Windows.Forms.ProgressBar();
            this.total_prg_lbl = new System.Windows.Forms.Label();
            this.start_btn = new System.Windows.Forms.Button();
            this.total_prg = new System.Windows.Forms.ProgressBar();
            this.result_tab_con = new System.Windows.Forms.TabControl();
            this.advice_tab = new System.Windows.Forms.TabPage();
            this.specific_ad_grp = new System.Windows.Forms.GroupBox();
            this.specific_ad_lbl = new System.Windows.Forms.Label();
            this.gen_ad_grp = new System.Windows.Forms.GroupBox();
            this.general_ad_lbl = new System.Windows.Forms.Label();
            this.introduction_ad_lbl = new System.Windows.Forms.Label();
            this.compare_tab = new System.Windows.Forms.TabPage();
            this.result_spl = new System.Windows.Forms.SplitContainer();
            this.compare_left_pnl = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.compare_left_type_cmb = new System.Windows.Forms.ComboBox();
            this.compare_left_item_cmb = new System.Windows.Forms.ComboBox();
            this.compare_right_pnl = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.compare_right_type_cmb = new System.Windows.Forms.ComboBox();
            this.compare_right_item_cmb = new System.Windows.Forms.ComboBox();
            this.view_tab = new System.Windows.Forms.TabPage();
            this.view_pnl = new System.Windows.Forms.Panel();
            this.result_tbl_view = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.view_type_cmb = new System.Windows.Forms.ComboBox();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.view_item_cmb = new System.Windows.Forms.ComboBox();
            this.save_view_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.general_spl)).BeginInit();
            this.general_spl.Panel1.SuspendLayout();
            this.general_spl.Panel2.SuspendLayout();
            this.general_spl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parameters_spl)).BeginInit();
            this.parameters_spl.Panel1.SuspendLayout();
            this.parameters_spl.Panel2.SuspendLayout();
            this.parameters_spl.SuspendLayout();
            this.prof_param_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.merge_class_trb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.perf_maint_trb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.merge_system_trb)).BeginInit();
            this.input_grp.SuspendLayout();
            this.result_tab_con.SuspendLayout();
            this.advice_tab.SuspendLayout();
            this.specific_ad_grp.SuspendLayout();
            this.gen_ad_grp.SuspendLayout();
            this.compare_tab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.result_spl)).BeginInit();
            this.result_spl.Panel1.SuspendLayout();
            this.result_spl.Panel2.SuspendLayout();
            this.result_spl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.view_tab.SuspendLayout();
            this.view_pnl.SuspendLayout();
            this.result_tbl_view.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.SuspendLayout();
            // 
            // general_spl
            // 
            this.general_spl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.general_spl.Location = new System.Drawing.Point(0, 0);
            this.general_spl.Name = "general_spl";
            // 
            // general_spl.Panel1
            // 
            this.general_spl.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.general_spl.Panel1.Controls.Add(this.parameters_spl);
            this.general_spl.Panel1.Padding = new System.Windows.Forms.Padding(10);
            this.general_spl.Panel1MinSize = 370;
            // 
            // general_spl.Panel2
            // 
            this.general_spl.Panel2.Controls.Add(this.result_tab_con);
            this.general_spl.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 10, 10);
            this.general_spl.Size = new System.Drawing.Size(1735, 694);
            this.general_spl.SplitterDistance = 370;
            this.general_spl.TabIndex = 2;
            // 
            // parameters_spl
            // 
            this.parameters_spl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameters_spl.IsSplitterFixed = true;
            this.parameters_spl.Location = new System.Drawing.Point(10, 10);
            this.parameters_spl.Margin = new System.Windows.Forms.Padding(10);
            this.parameters_spl.Name = "parameters_spl";
            this.parameters_spl.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // parameters_spl.Panel1
            // 
            this.parameters_spl.Panel1.Controls.Add(this.prof_param_grp);
            // 
            // parameters_spl.Panel2
            // 
            this.parameters_spl.Panel2.Controls.Add(this.input_grp);
            this.parameters_spl.Size = new System.Drawing.Size(350, 674);
            this.parameters_spl.SplitterDistance = 354;
            this.parameters_spl.TabIndex = 0;
            // 
            // prof_param_grp
            // 
            this.prof_param_grp.Controls.Add(this.merge_class_chk);
            this.prof_param_grp.Controls.Add(this.label6);
            this.prof_param_grp.Controls.Add(this.read_write_chk);
            this.prof_param_grp.Controls.Add(this.profile_cmb);
            this.prof_param_grp.Controls.Add(this.label1);
            this.prof_param_grp.Controls.Add(this.lock_chk);
            this.prof_param_grp.Controls.Add(this.label5);
            this.prof_param_grp.Controls.Add(this.blit_chk);
            this.prof_param_grp.Controls.Add(this.label2);
            this.prof_param_grp.Controls.Add(this.merge_class_trb);
            this.prof_param_grp.Controls.Add(this.order_chk);
            this.prof_param_grp.Controls.Add(this.label3);
            this.prof_param_grp.Controls.Add(this.label4);
            this.prof_param_grp.Controls.Add(this.merge_system_chk);
            this.prof_param_grp.Controls.Add(this.perf_maint_trb);
            this.prof_param_grp.Controls.Add(this.merge_system_trb);
            this.prof_param_grp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prof_param_grp.Location = new System.Drawing.Point(0, 0);
            this.prof_param_grp.Margin = new System.Windows.Forms.Padding(10);
            this.prof_param_grp.Name = "prof_param_grp";
            this.prof_param_grp.Size = new System.Drawing.Size(350, 354);
            this.prof_param_grp.TabIndex = 26;
            this.prof_param_grp.TabStop = false;
            this.prof_param_grp.Text = "Profile Parameters";
            // 
            // merge_class_chk
            // 
            this.merge_class_chk.AutoSize = true;
            this.merge_class_chk.Location = new System.Drawing.Point(14, 193);
            this.merge_class_chk.Name = "merge_class_chk";
            this.merge_class_chk.Size = new System.Drawing.Size(236, 17);
            this.merge_class_chk.TabIndex = 12;
            this.merge_class_chk.Text = "Merge components when they share classes";
            this.merge_class_chk.UseVisualStyleBackColor = true;
            this.merge_class_chk.CheckedChanged += new System.EventHandler(this.merge_class_chk_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(185, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Maintainance";
            // 
            // read_write_chk
            // 
            this.read_write_chk.AutoSize = true;
            this.read_write_chk.Location = new System.Drawing.Point(14, 170);
            this.read_write_chk.Name = "read_write_chk";
            this.read_write_chk.Size = new System.Drawing.Size(283, 17);
            this.read_write_chk.TabIndex = 16;
            this.read_write_chk.Text = "Split Read Access from Write Access (Recommended)";
            this.read_write_chk.UseVisualStyleBackColor = true;
            this.read_write_chk.CheckedChanged += new System.EventHandler(this.read_write_chk_CheckedChanged);
            // 
            // profile_cmb
            // 
            this.profile_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.profile_cmb.FormattingEnabled = true;
            this.profile_cmb.Location = new System.Drawing.Point(14, 89);
            this.profile_cmb.Margin = new System.Windows.Forms.Padding(10);
            this.profile_cmb.Name = "profile_cmb";
            this.profile_cmb.Size = new System.Drawing.Size(165, 21);
            this.profile_cmb.TabIndex = 7;
            this.profile_cmb.SelectedIndexChanged += new System.EventHandler(this.profile_cmb_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 248);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "0%";
            // 
            // lock_chk
            // 
            this.lock_chk.AutoSize = true;
            this.lock_chk.Location = new System.Drawing.Point(199, 91);
            this.lock_chk.Margin = new System.Windows.Forms.Padding(10);
            this.lock_chk.Name = "lock_chk";
            this.lock_chk.Size = new System.Drawing.Size(60, 17);
            this.lock_chk.TabIndex = 18;
            this.lock_chk.Text = "Default";
            this.lock_chk.UseVisualStyleBackColor = true;
            this.lock_chk.CheckedChanged += new System.EventHandler(this.lock_chk_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Performance";
            // 
            // blit_chk
            // 
            this.blit_chk.AutoSize = true;
            this.blit_chk.Location = new System.Drawing.Point(14, 146);
            this.blit_chk.Name = "blit_chk";
            this.blit_chk.Size = new System.Drawing.Size(218, 17);
            this.blit_chk.TabIndex = 15;
            this.blit_chk.Text = "Split Blittable from Non-Blittable Variables";
            this.blit_chk.UseVisualStyleBackColor = true;
            this.blit_chk.CheckedChanged += new System.EventHandler(this.blit_chk_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 248);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "100%";
            // 
            // merge_class_trb
            // 
            this.merge_class_trb.Location = new System.Drawing.Point(31, 216);
            this.merge_class_trb.Name = "merge_class_trb";
            this.merge_class_trb.Size = new System.Drawing.Size(148, 45);
            this.merge_class_trb.TabIndex = 9;
            this.merge_class_trb.Value = 5;
            this.merge_class_trb.Scroll += new System.EventHandler(this.merge_class_trb_Scroll);
            // 
            // order_chk
            // 
            this.order_chk.AutoSize = true;
            this.order_chk.Location = new System.Drawing.Point(14, 123);
            this.order_chk.Name = "order_chk";
            this.order_chk.Size = new System.Drawing.Size(216, 17);
            this.order_chk.TabIndex = 14;
            this.order_chk.Text = "Order variables on type (Recommended)";
            this.order_chk.UseVisualStyleBackColor = true;
            this.order_chk.CheckedChanged += new System.EventHandler(this.order_chk_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 322);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "0%";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(150, 322);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "100%";
            // 
            // merge_system_chk
            // 
            this.merge_system_chk.AutoSize = true;
            this.merge_system_chk.Location = new System.Drawing.Point(13, 267);
            this.merge_system_chk.Name = "merge_system_chk";
            this.merge_system_chk.Size = new System.Drawing.Size(220, 17);
            this.merge_system_chk.TabIndex = 13;
            this.merge_system_chk.Text = "Merge components if they share systems:";
            this.merge_system_chk.UseVisualStyleBackColor = true;
            this.merge_system_chk.CheckedChanged += new System.EventHandler(this.merge_system_chk_CheckedChanged);
            // 
            // perf_maint_trb
            // 
            this.perf_maint_trb.Location = new System.Drawing.Point(30, 26);
            this.perf_maint_trb.Margin = new System.Windows.Forms.Padding(10);
            this.perf_maint_trb.Maximum = 2;
            this.perf_maint_trb.Name = "perf_maint_trb";
            this.perf_maint_trb.Size = new System.Drawing.Size(203, 45);
            this.perf_maint_trb.TabIndex = 19;
            this.perf_maint_trb.Value = 1;
            this.perf_maint_trb.Scroll += new System.EventHandler(this.perf_maint_trb_Scroll);
            // 
            // merge_system_trb
            // 
            this.merge_system_trb.Location = new System.Drawing.Point(30, 290);
            this.merge_system_trb.Name = "merge_system_trb";
            this.merge_system_trb.Size = new System.Drawing.Size(149, 45);
            this.merge_system_trb.TabIndex = 17;
            this.merge_system_trb.Value = 5;
            this.merge_system_trb.Scroll += new System.EventHandler(this.merge_system_trb_Scroll);
            // 
            // input_grp
            // 
            this.input_grp.Controls.Add(this.project_cmb);
            this.input_grp.Controls.Add(this.application_lbl);
            this.input_grp.Controls.Add(this.plan_btn);
            this.input_grp.Controls.Add(this.search_btn);
            this.input_grp.Controls.Add(this.step_prg_lbl);
            this.input_grp.Controls.Add(this.prep_btn);
            this.input_grp.Controls.Add(this.step_prg);
            this.input_grp.Controls.Add(this.total_prg_lbl);
            this.input_grp.Controls.Add(this.start_btn);
            this.input_grp.Controls.Add(this.total_prg);
            this.input_grp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.input_grp.Location = new System.Drawing.Point(0, 0);
            this.input_grp.Margin = new System.Windows.Forms.Padding(10);
            this.input_grp.Name = "input_grp";
            this.input_grp.Size = new System.Drawing.Size(350, 316);
            this.input_grp.TabIndex = 13;
            this.input_grp.TabStop = false;
            this.input_grp.Text = "Input";
            // 
            // project_cmb
            // 
            this.project_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.project_cmb.FormattingEnabled = true;
            this.project_cmb.Location = new System.Drawing.Point(13, 41);
            this.project_cmb.Name = "project_cmb";
            this.project_cmb.Size = new System.Drawing.Size(257, 21);
            this.project_cmb.TabIndex = 11;
            this.project_cmb.SelectedIndexChanged += new System.EventHandler(this.project_cmb_SelectedIndexChanged);
            // 
            // application_lbl
            // 
            this.application_lbl.AutoSize = true;
            this.application_lbl.Location = new System.Drawing.Point(11, 25);
            this.application_lbl.Name = "application_lbl";
            this.application_lbl.Size = new System.Drawing.Size(62, 13);
            this.application_lbl.TabIndex = 10;
            this.application_lbl.Text = "Application:";
            // 
            // plan_btn
            // 
            this.plan_btn.Enabled = false;
            this.plan_btn.Location = new System.Drawing.Point(162, 249);
            this.plan_btn.Name = "plan_btn";
            this.plan_btn.Size = new System.Drawing.Size(94, 23);
            this.plan_btn.TabIndex = 9;
            this.plan_btn.Text = "Re-plan";
            this.plan_btn.UseVisualStyleBackColor = true;
            this.plan_btn.Click += new System.EventHandler(this.plan_btn_Click);
            // 
            // search_btn
            // 
            this.search_btn.Location = new System.Drawing.Point(276, 39);
            this.search_btn.Name = "search_btn";
            this.search_btn.Size = new System.Drawing.Size(57, 23);
            this.search_btn.TabIndex = 12;
            this.search_btn.Text = "Browse";
            this.search_btn.UseVisualStyleBackColor = true;
            this.search_btn.Click += new System.EventHandler(this.search_btn_Click);
            // 
            // step_prg_lbl
            // 
            this.step_prg_lbl.AutoSize = true;
            this.step_prg_lbl.Location = new System.Drawing.Point(13, 161);
            this.step_prg_lbl.Name = "step_prg_lbl";
            this.step_prg_lbl.Size = new System.Drawing.Size(76, 13);
            this.step_prg_lbl.TabIndex = 6;
            this.step_prg_lbl.Text = "Step Progress:";
            // 
            // prep_btn
            // 
            this.prep_btn.Location = new System.Drawing.Point(61, 249);
            this.prep_btn.Name = "prep_btn";
            this.prep_btn.Size = new System.Drawing.Size(95, 23);
            this.prep_btn.TabIndex = 0;
            this.prep_btn.Text = "Only Prepare";
            this.prep_btn.UseVisualStyleBackColor = true;
            this.prep_btn.Click += new System.EventHandler(this.prep_btn_Click);
            // 
            // step_prg
            // 
            this.step_prg.Location = new System.Drawing.Point(13, 184);
            this.step_prg.Margin = new System.Windows.Forms.Padding(10);
            this.step_prg.Name = "step_prg";
            this.step_prg.Size = new System.Drawing.Size(318, 23);
            this.step_prg.TabIndex = 5;
            // 
            // total_prg_lbl
            // 
            this.total_prg_lbl.AutoSize = true;
            this.total_prg_lbl.Location = new System.Drawing.Point(13, 93);
            this.total_prg_lbl.Name = "total_prg_lbl";
            this.total_prg_lbl.Size = new System.Drawing.Size(75, 13);
            this.total_prg_lbl.TabIndex = 7;
            this.total_prg_lbl.Text = "Total Progress";
            // 
            // start_btn
            // 
            this.start_btn.Location = new System.Drawing.Point(60, 220);
            this.start_btn.Name = "start_btn";
            this.start_btn.Size = new System.Drawing.Size(196, 23);
            this.start_btn.TabIndex = 8;
            this.start_btn.Text = "Start";
            this.start_btn.UseVisualStyleBackColor = true;
            this.start_btn.Click += new System.EventHandler(this.start_btn_Click);
            // 
            // total_prg
            // 
            this.total_prg.Location = new System.Drawing.Point(13, 116);
            this.total_prg.Margin = new System.Windows.Forms.Padding(10);
            this.total_prg.Maximum = 40;
            this.total_prg.Name = "total_prg";
            this.total_prg.Size = new System.Drawing.Size(318, 23);
            this.total_prg.Step = 4;
            this.total_prg.TabIndex = 4;
            // 
            // result_tab_con
            // 
            this.result_tab_con.Controls.Add(this.advice_tab);
            this.result_tab_con.Controls.Add(this.compare_tab);
            this.result_tab_con.Controls.Add(this.view_tab);
            this.result_tab_con.Dock = System.Windows.Forms.DockStyle.Fill;
            this.result_tab_con.Enabled = false;
            this.result_tab_con.Location = new System.Drawing.Point(0, 0);
            this.result_tab_con.Name = "result_tab_con";
            this.result_tab_con.SelectedIndex = 0;
            this.result_tab_con.Size = new System.Drawing.Size(1351, 684);
            this.result_tab_con.TabIndex = 1;
            // 
            // advice_tab
            // 
            this.advice_tab.Controls.Add(this.specific_ad_grp);
            this.advice_tab.Controls.Add(this.gen_ad_grp);
            this.advice_tab.Controls.Add(this.introduction_ad_lbl);
            this.advice_tab.Location = new System.Drawing.Point(4, 22);
            this.advice_tab.Name = "advice_tab";
            this.advice_tab.Padding = new System.Windows.Forms.Padding(3);
            this.advice_tab.Size = new System.Drawing.Size(1343, 658);
            this.advice_tab.TabIndex = 2;
            this.advice_tab.Text = "Advice";
            this.advice_tab.UseVisualStyleBackColor = true;
            // 
            // specific_ad_grp
            // 
            this.specific_ad_grp.AutoSize = true;
            this.specific_ad_grp.Controls.Add(this.specific_ad_lbl);
            this.specific_ad_grp.Location = new System.Drawing.Point(9, 395);
            this.specific_ad_grp.Name = "specific_ad_grp";
            this.specific_ad_grp.Size = new System.Drawing.Size(416, 158);
            this.specific_ad_grp.TabIndex = 2;
            this.specific_ad_grp.TabStop = false;
            this.specific_ad_grp.Text = "Framework Specific Advice";
            // 
            // specific_ad_lbl
            // 
            this.specific_ad_lbl.AutoSize = true;
            this.specific_ad_lbl.Location = new System.Drawing.Point(7, 29);
            this.specific_ad_lbl.Name = "specific_ad_lbl";
            this.specific_ad_lbl.Size = new System.Drawing.Size(35, 13);
            this.specific_ad_lbl.TabIndex = 1;
            this.specific_ad_lbl.Text = "label8";
            // 
            // gen_ad_grp
            // 
            this.gen_ad_grp.AutoSize = true;
            this.gen_ad_grp.Controls.Add(this.general_ad_lbl);
            this.gen_ad_grp.Location = new System.Drawing.Point(9, 62);
            this.gen_ad_grp.Name = "gen_ad_grp";
            this.gen_ad_grp.Size = new System.Drawing.Size(416, 158);
            this.gen_ad_grp.TabIndex = 1;
            this.gen_ad_grp.TabStop = false;
            this.gen_ad_grp.Text = "General ECS Design Advice";
            // 
            // general_ad_lbl
            // 
            this.general_ad_lbl.AutoSize = true;
            this.general_ad_lbl.Location = new System.Drawing.Point(7, 20);
            this.general_ad_lbl.Name = "general_ad_lbl";
            this.general_ad_lbl.Size = new System.Drawing.Size(35, 13);
            this.general_ad_lbl.TabIndex = 0;
            this.general_ad_lbl.Text = "label7";
            // 
            // introduction_ad_lbl
            // 
            this.introduction_ad_lbl.AutoSize = true;
            this.introduction_ad_lbl.Location = new System.Drawing.Point(6, 27);
            this.introduction_ad_lbl.Name = "introduction_ad_lbl";
            this.introduction_ad_lbl.Size = new System.Drawing.Size(447, 13);
            this.introduction_ad_lbl.TabIndex = 0;
            this.introduction_ad_lbl.Text = "This tab will show advice for creating an ECS design for your desired situation a" +
    "nd framework.";
            // 
            // compare_tab
            // 
            this.compare_tab.BackColor = System.Drawing.Color.White;
            this.compare_tab.Controls.Add(this.result_spl);
            this.compare_tab.Location = new System.Drawing.Point(4, 22);
            this.compare_tab.Name = "compare_tab";
            this.compare_tab.Padding = new System.Windows.Forms.Padding(3);
            this.compare_tab.Size = new System.Drawing.Size(1343, 658);
            this.compare_tab.TabIndex = 0;
            this.compare_tab.Text = "Compare";
            // 
            // result_spl
            // 
            this.result_spl.BackColor = System.Drawing.Color.Transparent;
            this.result_spl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.result_spl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.result_spl.Location = new System.Drawing.Point(3, 3);
            this.result_spl.Name = "result_spl";
            // 
            // result_spl.Panel1
            // 
            this.result_spl.Panel1.Controls.Add(this.compare_left_pnl);
            this.result_spl.Panel1.Controls.Add(this.splitContainer1);
            // 
            // result_spl.Panel2
            // 
            this.result_spl.Panel2.BackColor = System.Drawing.Color.White;
            this.result_spl.Panel2.Controls.Add(this.compare_right_pnl);
            this.result_spl.Panel2.Controls.Add(this.splitContainer2);
            this.result_spl.Size = new System.Drawing.Size(1337, 652);
            this.result_spl.SplitterDistance = 646;
            this.result_spl.TabIndex = 0;
            // 
            // compare_left_pnl
            // 
            this.compare_left_pnl.AutoScroll = true;
            this.compare_left_pnl.BackColor = System.Drawing.Color.Transparent;
            this.compare_left_pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compare_left_pnl.Location = new System.Drawing.Point(0, 32);
            this.compare_left_pnl.Name = "compare_left_pnl";
            this.compare_left_pnl.Size = new System.Drawing.Size(642, 616);
            this.compare_left_pnl.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.compare_left_type_cmb);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(5, 5, 2, 5);
            this.splitContainer1.Panel1MinSize = 75;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.compare_left_item_cmb);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(2, 5, 5, 5);
            this.splitContainer1.Panel2MinSize = 300;
            this.splitContainer1.Size = new System.Drawing.Size(642, 32);
            this.splitContainer1.SplitterDistance = 132;
            this.splitContainer1.TabIndex = 4;
            // 
            // compare_left_type_cmb
            // 
            this.compare_left_type_cmb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compare_left_type_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compare_left_type_cmb.FormattingEnabled = true;
            this.compare_left_type_cmb.Location = new System.Drawing.Point(5, 5);
            this.compare_left_type_cmb.Name = "compare_left_type_cmb";
            this.compare_left_type_cmb.Size = new System.Drawing.Size(125, 21);
            this.compare_left_type_cmb.TabIndex = 2;
            this.compare_left_type_cmb.SelectedIndexChanged += new System.EventHandler(this.compare_left_type_cmb_SelectedIndexChanged);
            // 
            // compare_left_item_cmb
            // 
            this.compare_left_item_cmb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compare_left_item_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compare_left_item_cmb.FormattingEnabled = true;
            this.compare_left_item_cmb.Location = new System.Drawing.Point(2, 5);
            this.compare_left_item_cmb.Name = "compare_left_item_cmb";
            this.compare_left_item_cmb.Size = new System.Drawing.Size(499, 21);
            this.compare_left_item_cmb.TabIndex = 3;
            this.compare_left_item_cmb.SelectedIndexChanged += new System.EventHandler(this.compare_left_item_cmb_SelectedIndexChanged);
            // 
            // compare_right_pnl
            // 
            this.compare_right_pnl.AutoScroll = true;
            this.compare_right_pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compare_right_pnl.Location = new System.Drawing.Point(0, 32);
            this.compare_right_pnl.Name = "compare_right_pnl";
            this.compare_right_pnl.Size = new System.Drawing.Size(683, 616);
            this.compare_right_pnl.TabIndex = 5;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.compare_right_type_cmb);
            this.splitContainer2.Panel1.Padding = new System.Windows.Forms.Padding(5, 5, 2, 5);
            this.splitContainer2.Panel1MinSize = 75;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.compare_right_item_cmb);
            this.splitContainer2.Panel2.Padding = new System.Windows.Forms.Padding(2, 5, 5, 5);
            this.splitContainer2.Panel2MinSize = 300;
            this.splitContainer2.Size = new System.Drawing.Size(683, 32);
            this.splitContainer2.SplitterDistance = 227;
            this.splitContainer2.TabIndex = 4;
            // 
            // compare_right_type_cmb
            // 
            this.compare_right_type_cmb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compare_right_type_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compare_right_type_cmb.FormattingEnabled = true;
            this.compare_right_type_cmb.Location = new System.Drawing.Point(5, 5);
            this.compare_right_type_cmb.Name = "compare_right_type_cmb";
            this.compare_right_type_cmb.Size = new System.Drawing.Size(220, 21);
            this.compare_right_type_cmb.TabIndex = 2;
            this.compare_right_type_cmb.SelectedIndexChanged += new System.EventHandler(this.compare_right_type_cmb_SelectedIndexChanged);
            // 
            // compare_right_item_cmb
            // 
            this.compare_right_item_cmb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compare_right_item_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compare_right_item_cmb.FormattingEnabled = true;
            this.compare_right_item_cmb.Location = new System.Drawing.Point(2, 5);
            this.compare_right_item_cmb.Name = "compare_right_item_cmb";
            this.compare_right_item_cmb.Size = new System.Drawing.Size(445, 21);
            this.compare_right_item_cmb.TabIndex = 3;
            this.compare_right_item_cmb.SelectedIndexChanged += new System.EventHandler(this.compare_right_item_cmb_SelectedIndexChanged);
            // 
            // view_tab
            // 
            this.view_tab.Controls.Add(this.view_pnl);
            this.view_tab.Controls.Add(this.splitContainer3);
            this.view_tab.Location = new System.Drawing.Point(4, 22);
            this.view_tab.Name = "view_tab";
            this.view_tab.Padding = new System.Windows.Forms.Padding(3);
            this.view_tab.Size = new System.Drawing.Size(1343, 658);
            this.view_tab.TabIndex = 1;
            this.view_tab.Text = "View";
            this.view_tab.UseVisualStyleBackColor = true;
            // 
            // view_pnl
            // 
            this.view_pnl.AutoScroll = true;
            this.view_pnl.Controls.Add(this.result_tbl_view);
            this.view_pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view_pnl.Location = new System.Drawing.Point(3, 36);
            this.view_pnl.Name = "view_pnl";
            this.view_pnl.Size = new System.Drawing.Size(1337, 619);
            this.view_pnl.TabIndex = 3;
            // 
            // result_tbl_view
            // 
            this.result_tbl_view.BackColor = System.Drawing.Color.White;
            this.result_tbl_view.ColumnCount = 2;
            this.result_tbl_view.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.result_tbl_view.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.result_tbl_view.Controls.Add(this.label7, 0, 2);
            this.result_tbl_view.Location = new System.Drawing.Point(5, 6);
            this.result_tbl_view.Name = "result_tbl_view";
            this.result_tbl_view.RowCount = 1;
            this.result_tbl_view.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.result_tbl_view.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.result_tbl_view.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.result_tbl_view.Size = new System.Drawing.Size(200, 200);
            this.result_tbl_view.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label7.Location = new System.Drawing.Point(3, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Hello";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.view_type_cmb);
            this.splitContainer3.Panel1.Padding = new System.Windows.Forms.Padding(5, 5, 2, 5);
            this.splitContainer3.Panel1MinSize = 100;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer3.Panel2.Padding = new System.Windows.Forms.Padding(2, 5, 5, 5);
            this.splitContainer3.Panel2MinSize = 300;
            this.splitContainer3.Size = new System.Drawing.Size(1337, 33);
            this.splitContainer3.SplitterDistance = 308;
            this.splitContainer3.TabIndex = 2;
            // 
            // view_type_cmb
            // 
            this.view_type_cmb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view_type_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.view_type_cmb.FormattingEnabled = true;
            this.view_type_cmb.Location = new System.Drawing.Point(5, 5);
            this.view_type_cmb.Name = "view_type_cmb";
            this.view_type_cmb.Size = new System.Drawing.Size(301, 21);
            this.view_type_cmb.TabIndex = 0;
            this.view_type_cmb.SelectedIndexChanged += new System.EventHandler(this.view_type_cmb_SelectedIndexChanged);
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.IsSplitterFixed = true;
            this.splitContainer4.Location = new System.Drawing.Point(2, 5);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.view_item_cmb);
            this.splitContainer4.Panel1MinSize = 250;
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.save_view_btn);
            this.splitContainer4.Panel2MinSize = 50;
            this.splitContainer4.Size = new System.Drawing.Size(1018, 23);
            this.splitContainer4.SplitterDistance = 926;
            this.splitContainer4.TabIndex = 0;
            // 
            // view_item_cmb
            // 
            this.view_item_cmb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view_item_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.view_item_cmb.FormattingEnabled = true;
            this.view_item_cmb.Location = new System.Drawing.Point(0, 0);
            this.view_item_cmb.Name = "view_item_cmb";
            this.view_item_cmb.Size = new System.Drawing.Size(926, 21);
            this.view_item_cmb.TabIndex = 1;
            this.view_item_cmb.SelectedIndexChanged += new System.EventHandler(this.view_item_cmb_SelectedIndexChanged);
            // 
            // save_view_btn
            // 
            this.save_view_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.save_view_btn.Enabled = false;
            this.save_view_btn.Location = new System.Drawing.Point(0, 0);
            this.save_view_btn.Margin = new System.Windows.Forms.Padding(0);
            this.save_view_btn.Name = "save_view_btn";
            this.save_view_btn.Size = new System.Drawing.Size(88, 23);
            this.save_view_btn.TabIndex = 2;
            this.save_view_btn.Text = "Save";
            this.save_view_btn.UseVisualStyleBackColor = true;
            this.save_view_btn.Click += new System.EventHandler(this.print_view_btn_Click);
            // 
            // MenuForm
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1735, 694);
            this.Controls.Add(this.general_spl);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "MenuForm";
            this.Text = "ENCODE Analysis Tool";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.general_spl.Panel1.ResumeLayout(false);
            this.general_spl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.general_spl)).EndInit();
            this.general_spl.ResumeLayout(false);
            this.parameters_spl.Panel1.ResumeLayout(false);
            this.parameters_spl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.parameters_spl)).EndInit();
            this.parameters_spl.ResumeLayout(false);
            this.prof_param_grp.ResumeLayout(false);
            this.prof_param_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.merge_class_trb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.perf_maint_trb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.merge_system_trb)).EndInit();
            this.input_grp.ResumeLayout(false);
            this.input_grp.PerformLayout();
            this.result_tab_con.ResumeLayout(false);
            this.advice_tab.ResumeLayout(false);
            this.advice_tab.PerformLayout();
            this.specific_ad_grp.ResumeLayout(false);
            this.specific_ad_grp.PerformLayout();
            this.gen_ad_grp.ResumeLayout(false);
            this.gen_ad_grp.PerformLayout();
            this.compare_tab.ResumeLayout(false);
            this.result_spl.Panel1.ResumeLayout(false);
            this.result_spl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.result_spl)).EndInit();
            this.result_spl.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.view_tab.ResumeLayout(false);
            this.view_pnl.ResumeLayout(false);
            this.result_tbl_view.ResumeLayout(false);
            this.result_tbl_view.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #region Edit Form

        public void UpdateTotalProgressBar(int value)
        {
            this.total_prg.Value = value;
        }

        public void UpdateStepProgressBar(int value)
        {
            this.step_prg.Value = value;
        }

        public void UpdateStepLabel(string value)
        {
            step_prg_lbl.Text = value;
            step_prg_lbl.Refresh();
        }
        #endregion

        #region Buttons
        private async void prep_btn_Click(object sender, EventArgs e)
        {
            this.start_btn.Enabled = false;
            this.prep_btn.Enabled = false;
            this.plan_btn.Enabled = false;
            ResetVisualization();

            await Program.PrepareProject();
            plan_btn.Enabled = true;

            this.start_btn.Enabled = true;
            this.prep_btn.Enabled = true;
            this.plan_btn.Enabled = true;
        }

        private async void start_btn_Click(object sender, EventArgs e)
        {
            this.start_btn.Enabled = false;
            this.prep_btn.Enabled = false;
            this.plan_btn.Enabled = false;
            ResetVisualization();

            await Program.PrepareProject();
            Program.PlanProject();
            this.plan_btn.Enabled = true;
            this.result_tab_con.Enabled = true;

            this.start_btn.Enabled = true;
            this.prep_btn.Enabled = true;
            this.plan_btn.Enabled = true;
        }

        private void plan_btn_Click(object sender, EventArgs e)
        {
            this.start_btn.Enabled = false;
            this.prep_btn.Enabled = false;
            this.plan_btn.Enabled = false;
            ResetVisualization();

            Program.projectStructure.ResetPlanningData();
            Program.PlanProject();

            this.start_btn.Enabled = true;
            this.prep_btn.Enabled = true;
            this.plan_btn.Enabled = true;
        }


        private void search_btn_Click(object sender, EventArgs e)
        {
            Thread staThread = new Thread(new ThreadStart(SearchProjectFile));
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            project_cmb.Text = Program.projectPath;
        }

        private void SearchProjectFile()
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "C# project files (*.csproj)|*.csproj";
                openFileDialog.FilterIndex = 2;
                //openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                }
            }
            Program.projectPath = filePath;
        }

        private void print_view_btn_Click(object sender, EventArgs e)
        {
            if (this.view_pnl.Controls.Count != 0)
                PrintViewDesign((TableLayoutPanel)this.view_pnl.Controls[0]);
        }

        #endregion

        #region Checkboxes
        private void lock_chk_CheckedChanged(object sender, EventArgs e)
        {
            order_chk.Enabled = !lock_chk.Checked;
            blit_chk.Enabled = !lock_chk.Checked;
            read_write_chk.Enabled = !lock_chk.Checked;
            merge_class_chk.Enabled = !lock_chk.Checked;
            merge_class_trb.Enabled = !lock_chk.Checked && merge_class_chk.Checked;
            merge_system_chk.Enabled = !lock_chk.Checked;
            merge_system_trb.Enabled = !lock_chk.Checked && merge_system_chk.Checked;
        }

        private void order_chk_CheckedChanged(object sender, EventArgs e)
        {
            if (changingPlanProfile)
                return;

            Program.planProfile.OrderVariableTypes = order_chk.Checked;

            profile_cmb.SelectedItem = "Custom";
        }

        private void blit_chk_CheckedChanged(object sender, EventArgs e)
        {
            if (changingPlanProfile)
                return;

            Program.planProfile.SplitOnBlittable = blit_chk.Checked;

            profile_cmb.SelectedItem = "Custom";
        }

        private void read_write_chk_CheckedChanged(object sender, EventArgs e)
        {
            if (changingPlanProfile)
                return;

            Program.planProfile.SplitOnReadWriteAccess = read_write_chk.Checked;

            profile_cmb.SelectedItem = "Custom";
        }

        private void merge_class_chk_CheckedChanged(object sender, EventArgs e)
        {
            if (changingPlanProfile)
                return;

            merge_class_trb.Enabled = merge_class_chk.Checked;
            if (merge_class_chk.Checked)
                Program.planProfile.MergeOnClassEqualityLevel = ((float)merge_class_trb.Value) / 10f;
            else
                Program.planProfile.MergeOnClassEqualityLevel = -1f;

            profile_cmb.SelectedItem = "Custom";
        }

        private void merge_system_chk_CheckedChanged(object sender, EventArgs e)
        {
            if (changingPlanProfile)
                return;

            merge_system_trb.Enabled = merge_system_chk.Checked;
            if (merge_system_chk.Checked)
                Program.planProfile.MergeOnSystemEqualityLevel = ((float)merge_system_trb.Value) / 10f;
            else
                Program.planProfile.MergeOnSystemEqualityLevel = -1f;

            profile_cmb.SelectedItem = "Custom";
        }

        #endregion

        #region Trackbars
        private void merge_class_trb_Scroll(object sender, EventArgs e)
        {
            if (changingPlanProfile)
                return;

            if (merge_class_chk.Checked)
                Program.planProfile.MergeOnClassEqualityLevel = ((float)merge_class_trb.Value) / 10f;

            profile_cmb.Text = "Custom";
        }

        private void merge_system_trb_Scroll(object sender, EventArgs e)
        {
            if (changingPlanProfile)
                return;

            if (merge_system_chk.Checked)
                Program.planProfile.MergeOnSystemEqualityLevel = ((float)merge_system_trb.Value) / 10f;

            profile_cmb.Text = "Custom";
        }

        private void perf_maint_trb_Scroll(object sender, EventArgs e)
        {
            Program.planProfile.ChangePerformanceOrientation(perf_maint_trb.Value);
            UploadProfileParameters();
        }
        #endregion

        #region Comboboxes
        private void profile_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (changingPlanProfile)
                return;

            Program.planProfile.ChangeToPlanProfile(profile_cmb.Text.ToString());

            if (profile_cmb.Text == "Custom")
                lock_chk.Checked = false;
            else
                lock_chk.Checked = true;

            UploadProfileParameters();
            ChangeAdvice();
        }

        private void UploadProfileParameters()
        {
            changingPlanProfile = true;

            order_chk.Checked = Program.planProfile.OrderVariableTypes;
            blit_chk.Checked = Program.planProfile.SplitOnBlittable;
            read_write_chk.Checked = Program.planProfile.SplitOnReadWriteAccess;
            merge_class_trb.Value = Math.Max((int)(Program.planProfile.MergeOnClassEqualityLevel * 10), 0);
            merge_class_chk.Checked = Program.planProfile.MergeOnClassEqualityLevel >= 0;
            merge_system_trb.Value = Math.Max((int)(Program.planProfile.MergeOnSystemEqualityLevel * 10), 0);
            merge_system_chk.Checked = Program.planProfile.MergeOnSystemEqualityLevel >= 0;

            profile_cmb.SelectedItem = Program.planProfile.ProfileName;
            changingPlanProfile = false;
        }

        private void ChangeAdvice()
        {
            this.specific_ad_grp.Text = $"Framework Specific Advice: {profile_cmb.Text}";


            // Add general advice
            string specificAdvice = string.Empty;
            foreach (string advice in Program.planProfile.SpecificPlanningRules)
            {
                specificAdvice += $"- {advice} \n \n";
            }
            this.specific_ad_lbl.Text = specificAdvice;
        }

        private void project_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.projectPath = project_cmb.SelectedItem.ToString();
        }

        private void compare_left_type_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmbItemObjects = GetDocItems(compare_left_type_cmb.SelectedItem.ToString(), out int noItems);

            this.compare_left_item_cmb.Items.Clear();
            this.compare_left_item_cmb.Text = $"({noItems} {compare_left_type_cmb.SelectedItem}((e)s))";
            this.compare_left_item_cmb.Items.AddRange(cmbItemObjects);

        }

        private void compare_right_type_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmbItemObjects = GetDocItems(compare_right_type_cmb.SelectedItem.ToString(), out int noItems);

            this.compare_right_item_cmb.Items.Clear();
            this.compare_right_item_cmb.Text = $"({noItems} {compare_right_type_cmb.SelectedItem}((e)s))";
            this.compare_right_item_cmb.Items.AddRange(cmbItemObjects);

        }

        private void view_type_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmbItemObjects = GetDocItems(view_type_cmb.SelectedItem.ToString(), out int noItems);

            this.view_item_cmb.Items.Clear();
            this.view_item_cmb.Text = $"({noItems} {view_type_cmb.SelectedItem}((e)s))";

            this.view_item_cmb.Items.Add("All");
            this.view_item_cmb.Items.AddRange(cmbItemObjects);
        }

        private object[] GetDocItems(string selectedItem, out int noItems)
        {
            object[] cmbItemObjects = new object[0];
            noItems = 0;

            switch (selectedItem)
            {
                case nameof(Types.File):
                    List<OODFile> oodFiles = Program.projectStructure.oodFiles;

                    noItems = oodFiles.Count;
                    cmbItemObjects = new object[oodFiles.Count];
                    for (int i = 0; i < oodFiles.Count; i++)
                    {
                        //if(oodFiles[i].rootTreeNumber != -1)
                        cmbItemObjects[i] = $"{i}. {oodFiles[i].GetLabel()}";
                    }
                    break;
                case nameof(Types.Namespace):
                    List<OODNamespace> oodNamespaces = Program.projectStructure.oodNamespaces;

                    noItems = oodNamespaces.Count;
                    cmbItemObjects = new object[oodNamespaces.Count];
                    for (int i = 0; i < oodNamespaces.Count; i++)
                    {
                        cmbItemObjects[i] = $"{i}. {oodNamespaces[i].GetLabel()}";
                    }
                    break;
                case nameof(Types.Class):
                    List<OODClass> oodClasses = Program.projectStructure.oodClasses;

                    noItems = oodClasses.Count;
                    cmbItemObjects = new object[oodClasses.Count];
                    for (int i = 0; i < oodClasses.Count; i++)
                    {
                        cmbItemObjects[i] = $"{i}. {oodClasses[i].GetLabel()}";
                    }
                    break;
                case nameof(Types.Method):
                    List<OODMethod> oodMethods = Program.projectStructure.oodMethods;

                    noItems = oodMethods.Count;
                    cmbItemObjects = new object[oodMethods.Count];
                    for (int i = 0; i < oodMethods.Count; i++)
                    {
                        cmbItemObjects[i] = $"{i}. {oodMethods[i].GetLabel()}";
                    }
                    break;
                case nameof(Types.Entity):
                    List<ECSEntityType> ecsEntities = Program.projectStructure.ecsEntityTypes;

                    noItems = ecsEntities.Count;
                    cmbItemObjects = new object[ecsEntities.Count];
                    for (int i = 0; i < ecsEntities.Count; i++)
                    {
                        cmbItemObjects[i] = $"{i}. {ecsEntities[i].GetLabel()}";
                    }
                    break;
                case nameof(Types.Component):
                    List<ECSComponent> ecsComponents = Program.projectStructure.ecsComponents;

                    noItems = ecsComponents.Count;
                    cmbItemObjects = new object[ecsComponents.Count];
                    for (int i = 0; i < ecsComponents.Count; i++)
                    {
                        cmbItemObjects[i] = $"{i}. {ecsComponents[i].GetLabel()}";
                    }
                    break;
                case nameof(Types.System):
                    List<ECSSystem> ecsSystems = Program.projectStructure.ecsSystems;

                    noItems = ecsSystems.Count;
                    cmbItemObjects = new object[ecsSystems.Count];
                    for (int i = 0; i < ecsSystems.Count; i++)
                    {
                        cmbItemObjects[i] = $"{i}. {ecsSystems[i].GetLabel()}";
                    }
                    break;
                default:
                    break;
            }
            return cmbItemObjects;
        }

        private void compare_left_item_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            compare_left_type_cmb.Text = compare_left_type_cmb.SelectedItem.ToString();

            int typeLeftIndex = GetTypeIndex(compare_left_type_cmb.SelectedItem.ToString());
            int itemLeftIndex = compare_left_item_cmb.SelectedIndex;
            IndexTuple leftIndexTuple = new IndexTuple(typeLeftIndex, itemLeftIndex);

            // Left Panel
            UpdateLeftPanel(leftIndexTuple);

            // Right Panel
            this.compare_right_pnl.Controls.RemoveByKey("result_tbl_right");
            IndexTuple rightIndexTuple = Program.projectStructure.GetCoupleFromDictionary(leftIndexTuple);

            if (rightIndexTuple.IsValid())
                UpdateRightPanel(rightIndexTuple);
        }

        private void compare_right_item_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            compare_right_item_cmb.Text = compare_right_item_cmb.SelectedItem.ToString();

            int typeRightIndex = GetTypeIndex(compare_right_type_cmb.SelectedItem.ToString());
            int itemRightIndex = compare_right_item_cmb.SelectedIndex;
            IndexTuple rightIndexTuple = new IndexTuple(typeRightIndex, itemRightIndex);

            // Right Panel
            UpdateRightPanel(rightIndexTuple);

            // Left Panel
            this.compare_left_pnl.Controls.RemoveByKey("result_tbl_left");
            IndexTuple leftIndexTuple = Program.projectStructure.GetCoupleFromDictionary(rightIndexTuple);

            if (leftIndexTuple.IsValid())
                UpdateLeftPanel(leftIndexTuple);
        }

        private void view_item_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.view_pnl.Controls.RemoveByKey("result_tbl_view");
            view_type_cmb.Text = view_type_cmb.SelectedItem.ToString();

            int typeIndex = GetTypeIndex(view_type_cmb.SelectedItem.ToString());
            List<List<QueryResult>> queryResults;

            if (view_item_cmb.SelectedIndex == 0)
            {
                queryResults = GetQueryResultAll(typeIndex);
            }
            else
            {
                int itemIndex = GetItemIndex(view_item_cmb.SelectedIndex);
                IndexTuple indexTuple = new IndexTuple(typeIndex, itemIndex);

                queryResults = GetQueryResult(indexTuple, out List<List<QueryResult>> altQueryResults);
                queryResults.AddRange(altQueryResults);
            }

            TableLayoutPanel result_tbl = CreateTableLayout(queryResults, "view");
            result_tbl.BackColor = System.Drawing.Color.White;
            this.view_pnl.Controls.Add(result_tbl);

            NormalizeComponentWidth(this.view_pnl);

            this.save_view_btn.Enabled = true;
        }

        private int GetItemIndex(int selectedIndex)
        {
            return selectedIndex - 1;
        }

        private int GetTypeIndex(string selectedItem)
        {
            switch (selectedItem)
            {
                case nameof(Types.File):
                    return (int)Types.File;
                case nameof(Types.Namespace):
                    return (int)Types.Namespace;
                case nameof(Types.Class):
                    return (int)Types.Class;
                case nameof(Types.Method):
                    return (int)Types.Method;
                case nameof(Types.Entity):
                    return (int)Types.Entity;
                case nameof(Types.Component):
                    return (int)Types.Component;
                case nameof(Types.System):
                    return (int)Types.System;
                default:
                    return -1;
            }
        }

        #endregion

        #region Table Creation

        private void UpdateRightPanel(IndexTuple rightIndexTuple)
        {
            this.compare_right_pnl.Controls.RemoveByKey("result_tbl_right");

            List<List<QueryResult>> rightQueryResults = GetQueryResult(rightIndexTuple, out _);
            TableLayoutPanel right_tbl = CreateTableLayout(rightQueryResults, "right");
            this.compare_right_pnl.Controls.Add(right_tbl);

            NormalizeComponentWidth(this.compare_right_pnl);
        }

        private void UpdateLeftPanel(IndexTuple leftIndexTuple)
        {
            this.compare_left_pnl.Controls.RemoveByKey("result_tbl_left");

            List<List<QueryResult>> leftQueryResults = GetQueryResult(leftIndexTuple, out _);
            TableLayoutPanel left_tbl = CreateTableLayout(leftQueryResults, "left");
            this.compare_left_pnl.Controls.Add(left_tbl);

            NormalizeComponentWidth(this.compare_left_pnl);
        }

        private List<List<QueryResult>> GetQueryResult(IndexTuple indexTuple, out List<List<QueryResult>> alternativeResultQuery)
        {
            switch (indexTuple.arrayIndex)
            {
                case (int)Types.Class:
                    alternativeResultQuery = new List<List<QueryResult>>();
                    return TreeWalker.DrawClass(indexTuple, Program.projectStructure);
                case (int)Types.Method:
                    alternativeResultQuery = new List<List<QueryResult>>();
                    return TreeWalker.DrawMethod(indexTuple, Program.projectStructure);
                case (int)Types.Entity:
                    // Prepare all links too
                    HashSet<IndexTuple> usedInSystemsHash = Program.projectStructure.ecsEntityTypes[indexTuple.itemIndex].usedInSystems;
                    alternativeResultQuery = new List<List<QueryResult>>();
                    foreach (var systemIndex in usedInSystemsHash)
                    {
                        alternativeResultQuery.AddRange(TreeWalker.DrawSystem(systemIndex, Program.projectStructure));
                    }

                    return TreeWalker.DrawEntity(indexTuple, Program.projectStructure);
                case (int)Types.Component:
                    // Prepare all links too
                    List<IndexTuple> usedInSystemsList = Program.projectStructure.ecsComponents[indexTuple.itemIndex].readInECSSystems.Union(Program.projectStructure.ecsComponents[indexTuple.itemIndex].writenInECSSystems).ToList();
                    alternativeResultQuery = new List<List<QueryResult>>();
                    foreach (var systemIndex in usedInSystemsList)
                    {
                        alternativeResultQuery.AddRange(TreeWalker.DrawSystem(systemIndex, Program.projectStructure));
                    }

                    return TreeWalker.DrawComponent(indexTuple, Program.projectStructure);
                case (int)Types.System:
                    alternativeResultQuery = new List<List<QueryResult>>();
                    return TreeWalker.DrawSystem(indexTuple, Program.projectStructure);
                default:
                    alternativeResultQuery = new List<List<QueryResult>>();
                    return new List<List<QueryResult>>();
            }
        }

        private List<List<QueryResult>> GetQueryResultAll(int typeIndex)
        {
            List<List<QueryResult>> resultQuery = new List<List<QueryResult>>();

            switch (typeIndex)
            {
                //case (int)Types.File:
                //    break;
                //case (int)Types.Namespace:
                //    break;
                case (int)Types.Class:

                    for (int entityIndex = 0; entityIndex < Program.projectStructure.oodClasses.Count; entityIndex++)
                    {
                        resultQuery.AddRange(TreeWalker.DrawClass(new IndexTuple(typeIndex, entityIndex), Program.projectStructure));
                    }

                    return resultQuery;
                case (int)Types.Entity:

                    for (int entityIndex = 0; entityIndex < Program.projectStructure.ecsEntityTypes.Count; entityIndex++)
                    {
                        resultQuery.AddRange(TreeWalker.DrawEntityGrid(new IndexTuple(typeIndex, entityIndex), Program.projectStructure));
                    }

                    return resultQuery;
                case (int)Types.System:

                    for (int entityIndex = 0; entityIndex < Program.projectStructure.ecsSystems.Count; entityIndex++)
                    {
                        resultQuery.AddRange(TreeWalker.DrawSystem(new IndexTuple(typeIndex, entityIndex), Program.projectStructure));
                    }

                    return resultQuery;
                //case (int)Types.ClassGroup:
                //    break;
                //case (int)Types.MethodGroup:
                //    break;
                default:
                    return new List<List<QueryResult>>();
            }
        }

        private TableLayoutPanel CreateTableLayout(List<List<QueryResult>> colRow, string side)
        {
            TableLayoutPanel result_tbl;
            result_tbl = new System.Windows.Forms.TableLayoutPanel();
            //result_tbl.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            result_tbl.Location = new System.Drawing.Point(0, 32);
            result_tbl.Name = $"result_tbl_{side}";
            result_tbl.Size = new System.Drawing.Size(12, 12);
            result_tbl.Padding = new System.Windows.Forms.Padding(10);
            result_tbl.TabIndex = 2;
            result_tbl.AutoScroll = true;

            CreateRowsAndColumns(colRow, ref result_tbl);

            return result_tbl;
        }

        private void CreateRowsAndColumns(List<List<QueryResult>> colRow, ref TableLayoutPanel panel)
        {
            if (colRow.Count == 0)
                return;

            panel.ColumnCount = colRow.Count;

            int maxRows = colRow[0].Count;
            foreach (var row in colRow)
                maxRows = Math.Max(maxRows, row.Count);

            panel.RowCount = maxRows;
            panel.AutoSize = true;

            // Add default size columns
            for (int i = 0; i < panel.ColumnCount; i++)
            {
                panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize, 45F));
            }

            // Add default size rows
            for (int i = 0; i < panel.RowCount; i++)
            {
                panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            }

            for (int i = 0; i < panel.ColumnCount; i++)
            {
                // Create the Entity Tag
                QueryResult result = colRow[i][0];
                System.Windows.Forms.Label entity_lbl = new System.Windows.Forms.Label();

                entity_lbl.AutoSize = true;
                entity_lbl.Location = new System.Drawing.Point(3, 10);
                entity_lbl.Name = $"{result.indexTuple}_lbl";
                entity_lbl.Size = new System.Drawing.Size(35, 13);
                entity_lbl.TabIndex = 0;
                entity_lbl.Text = result.name;
                entity_lbl.Enabled = result.enabled;
                panel.RowStyles[0].Height = entity_lbl.Size.Height;

                panel.Controls.Add(entity_lbl, i, 0);

                int widestComponent = 90;

                // Add all components
                for (int j = 1; j < colRow[i].Count; j++)
                {
                    result = colRow[i][j];
                    System.Windows.Forms.GroupBox temp_grp = new System.Windows.Forms.GroupBox();
                    temp_grp.SuspendLayout();

                    temp_grp.Location = new System.Drawing.Point(3, 3);
                    temp_grp.Name = $"{result.indexTuple}_grp";
                    temp_grp.AutoSize = true;
                    temp_grp.AutoSizeMode = AutoSizeMode.GrowOnly;
                    temp_grp.TabIndex = 0;
                    temp_grp.TabStop = false;
                    temp_grp.Text = result.name;

                    int startHeight = 20;
                    for (int k = 0; k < result.value.Count; k++)
                    {
                        string newLabel = result.value[k];

                        System.Windows.Forms.Label temp_lbl = new System.Windows.Forms.Label();
                        temp_lbl.AutoSize = true;
                        temp_lbl.Location = new System.Drawing.Point(3, 20 + k * startHeight);
                        temp_lbl.Name = $"{result.indexTuple}_{k}_lbl";
                        temp_lbl.Size = new System.Drawing.Size(90, 13);
                        temp_lbl.TabIndex = 0;
                        temp_lbl.Text = newLabel;
                        temp_grp.Controls.Add(temp_lbl);
                    }

                    temp_grp.Enabled = result.enabled;
                    temp_grp.Size = new System.Drawing.Size(widestComponent, result.value.Count * 20 + startHeight + 5);
                    temp_grp.MinimumSize = new System.Drawing.Size(widestComponent, result.value.Count * 20 + startHeight + 5);

                    temp_grp.ResumeLayout(false);
                    temp_grp.PerformLayout();

                    // If groupbox is too big, alter
                    if (temp_grp.Size.Width > panel.ColumnStyles[i].Width)
                    {
                        panel.ColumnStyles[i].Width = temp_grp.Size.Width;
                        widestComponent = temp_grp.MinimumSize.Width;
                    }
                    //else
                    //    groupBox1.Size = new System.Drawing.Size((int)panel.ColumnStyles[i].Width, groupBox1.Size.Height);

                    if (temp_grp.Size.Height > panel.RowStyles[j].Height)
                        panel.RowStyles[j].Height = temp_grp.Size.Height;

                    panel.Controls.Add(temp_grp, i, j);
                }

            }
        }

        private void NormalizeComponentWidth(System.Windows.Forms.Control parent) // TODO: Check this
        {
            //if (parent.GetType() != typeof(SplitterPanel))
            //    return;

            var control = parent.Controls[parent.Controls.Count - 1];

            if (control.GetType() != typeof(TableLayoutPanel))
                return;

            var panel = (TableLayoutPanel)control;
            int maxWidth = 190;

            // Find widest component
            for (int item = 0; item < panel.Controls.Count; item++)
            {
                if (panel.Controls[item].GetType() != typeof(System.Windows.Forms.GroupBox))
                    continue;

                maxWidth = Math.Max(maxWidth, ((System.Windows.Forms.GroupBox)panel.Controls[item]).Size.Width);
            }

            // Make all components the same width
            for (int item = 0; item < panel.Controls.Count; item++)
            {
                if (panel.Controls[item].GetType() != typeof(System.Windows.Forms.GroupBox))
                    continue;

                System.Windows.Forms.GroupBox groupBox = (System.Windows.Forms.GroupBox)panel.Controls[item];
                groupBox.MinimumSize = new System.Drawing.Size(maxWidth, groupBox.MinimumSize.Height);
            }
        }
        #endregion

        private void ResetVisualization()
        {
            this.save_view_btn.Enabled = false;

            compare_left_type_cmb.SelectedIndex = compare_left_type_cmb.SelectedIndex;
            compare_right_type_cmb.SelectedIndex = compare_right_type_cmb.SelectedIndex;
            view_type_cmb.SelectedIndex = view_type_cmb.SelectedIndex;

            this.compare_left_pnl.Controls.RemoveByKey("result_tbl_left");
            this.compare_right_pnl.Controls.RemoveByKey("result_tbl_right");
            this.view_pnl.Controls.RemoveByKey("result_tbl_view");
        }

        #region Print

        public void PrintViewDesign(TableLayoutPanel panel)
        {
            using (Graphics gfx = panel.CreateGraphics())
            {
                using (Bitmap bitmap = new Bitmap(panel.Width, panel.Height, gfx))
                {
                    panel.DrawToBitmap(bitmap, new Rectangle(0, 0, panel.Width, panel.Height));

                    string[] project = this.project_cmb.SelectedItem.ToString().Split('\\');
                    string settings = $"{perf_maint_trb.Value}{(order_chk.Checked ? "T" : "F")}{(blit_chk.Checked ? "T" : "F")}{(read_write_chk.Checked ? "T" : "F")}{(merge_class_chk.Checked ? merge_class_trb.Value.ToString() : "F")}{(merge_system_chk.Checked ? merge_system_trb.Value.ToString() : "F")}";
                    string baseName = $"{project[project.Length - 2]}_{this.view_type_cmb.SelectedItem}_{this.view_item_cmb.SelectedItem}_{settings}";
                    string folder = $"{project[project.Length - 2]}\\{this.profile_cmb.Text}";

                    //Thread staThread = new Thread(new ThreadStart(SaveImage));
                    Thread staThread = new Thread(() => SaveImage(bitmap, folder, baseName));
                    staThread.SetApartmentState(ApartmentState.STA);
                    staThread.Start();
                    staThread.Join();

                }
            }
        }

        private void SaveImage(Bitmap bitmap, string folder, string baseName)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Jpeg Image|*.jpg";
                saveFileDialog.Title = "Save an Image File";
                saveFileDialog.FileName = baseName;
                saveFileDialog.InitialDirectory = $@"D:\Plastic\XVR.one\XVR.DOCAnalysisTool\Graphs\{folder}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bitmap.Save(saveFileDialog.FileName);
                }
            }
        }


        #endregion

    }
}

