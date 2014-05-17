namespace DynamicProxies.Client.Windows
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.testProxy = new System.Windows.Forms.Button();
			this.callCount = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// testProxy
			// 
			this.testProxy.Location = new System.Drawing.Point(12, 12);
			this.testProxy.Name = "testProxy";
			this.testProxy.Size = new System.Drawing.Size(75, 23);
			this.testProxy.TabIndex = 0;
			this.testProxy.Text = "&Test Proxy";
			this.testProxy.UseVisualStyleBackColor = true;
			this.testProxy.Click += new System.EventHandler(this.OnTestProxyClick);
			// 
			// callCount
			// 
			this.callCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.callCount.Location = new System.Drawing.Point(13, 42);
			this.callCount.Name = "callCount";
			this.callCount.Size = new System.Drawing.Size(100, 23);
			this.callCount.TabIndex = 1;
			this.callCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 77);
			this.Controls.Add(this.callCount);
			this.Controls.Add(this.testProxy);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Dynamic Proxies Demo";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button testProxy;
		private System.Windows.Forms.Label callCount;
	}
}

