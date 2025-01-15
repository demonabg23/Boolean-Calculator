namespace Logical_Expression_Interpreter_UI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Builder = new Button();
            panel1 = new Panel();
            label1 = new Label();
            textBoxFuncName = new TextBox();
            DefineButton = new Button();
            textBoxDefinition = new TextBox();
            SuspendLayout();
            // 
            // Builder
            // 
            Builder.Location = new Point(67, 60);
            Builder.Name = "Builder";
            Builder.Size = new Size(75, 22);
            Builder.TabIndex = 0;
            Builder.Text = "Build";
            Builder.UseVisualStyleBackColor = true;
            Builder.Click += Builder_Click;
            // 
            // panel1
            // 
            panel1.Location = new Point(40, 123);
            panel1.Name = "panel1";
            panel1.Size = new Size(731, 308);
            panel1.TabIndex = 1;
            panel1.Paint += panel1_Paint;
            // 
            // label1
            // 
            label1.Location = new Point(477, 9);
            label1.Name = "label1";
            label1.Size = new Size(294, 78);
            label1.TabIndex = 4;
            // 
            // textBoxFuncName
            // 
            textBoxFuncName.Location = new Point(174, 61);
            textBoxFuncName.Name = "textBoxFuncName";
            textBoxFuncName.Size = new Size(264, 23);
            textBoxFuncName.TabIndex = 2;
            // 
            // DefineButton
            // 
            DefineButton.Location = new Point(67, 16);
            DefineButton.Name = "DefineButton";
            DefineButton.Size = new Size(75, 23);
            DefineButton.TabIndex = 3;
            DefineButton.Text = "Define";
            DefineButton.UseVisualStyleBackColor = true;
            DefineButton.Click += DefineButton_Click;
            // 
            // textBoxDefinition
            // 
            textBoxDefinition.Location = new Point(174, 16);
            textBoxDefinition.Name = "textBoxDefinition";
            textBoxDefinition.Size = new Size(264, 23);
            textBoxDefinition.TabIndex = 5;
            textBoxDefinition.TextChanged += textBoxDefinition_TextChanged;
            // 
            // Form1
            // 
            ClientSize = new Size(783, 557);
            Controls.Add(label1);
            Controls.Add(textBoxDefinition);
            Controls.Add(DefineButton);
            Controls.Add(textBoxFuncName);
            Controls.Add(panel1);
            Controls.Add(Builder);
            Name = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Builder;
        private Panel panel1;
        private TextBox textBoxFuncName;
        private Button DefineButton;
        private Label label1;
        private TextBox textBoxDefinition;
    }
}
