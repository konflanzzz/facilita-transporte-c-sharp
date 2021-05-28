
namespace facilitaTransporte
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
            this.emitirDocumento = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.procurarDocumentos = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.lerArquivoXML = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // emitirDocumento
            // 
            this.emitirDocumento.Location = new System.Drawing.Point(164, 290);
            this.emitirDocumento.Name = "emitirDocumento";
            this.emitirDocumento.Size = new System.Drawing.Size(126, 40);
            this.emitirDocumento.TabIndex = 0;
            this.emitirDocumento.Text = "Emitir Documento";
            this.emitirDocumento.UseVisualStyleBackColor = true;
            this.emitirDocumento.Click += new System.EventHandler(this.emitirDocumento_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(596, 23);
            this.textBox1.TabIndex = 1;
            // 
            // procurarDocumentos
            // 
            this.procurarDocumentos.Location = new System.Drawing.Point(13, 290);
            this.procurarDocumentos.Name = "procurarDocumentos";
            this.procurarDocumentos.Size = new System.Drawing.Size(145, 40);
            this.procurarDocumentos.TabIndex = 2;
            this.procurarDocumentos.Text = "Procurar Documentos";
            this.procurarDocumentos.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(13, 43);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(596, 23);
            this.textBox2.TabIndex = 3;
            // 
            // lerArquivoXML
            // 
            this.lerArquivoXML.Location = new System.Drawing.Point(297, 290);
            this.lerArquivoXML.Name = "lerArquivoXML";
            this.lerArquivoXML.Size = new System.Drawing.Size(130, 40);
            this.lerArquivoXML.TabIndex = 4;
            this.lerArquivoXML.Text = "Ler Arquivo XML";
            this.lerArquivoXML.UseVisualStyleBackColor = true;
            this.lerArquivoXML.Click += new System.EventHandler(this.lerArquivoXML_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 346);
            this.Controls.Add(this.lerArquivoXML);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.procurarDocumentos);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.emitirDocumento);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button emitirDocumento;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button procurarDocumentos;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button lerArquivoXML;
    }
}

