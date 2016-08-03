namespace PokemonGo.RocketAPI.GUI
{
    partial class PokemonForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokemonForm));
            this.pokemonListView = new System.Windows.Forms.ListView();
            this.pokemonListMenuStrip = new System.Windows.Forms.MenuStrip();
            this.sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByCPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByIVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transferSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evolveSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.powerupSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbPokeListLoading = new System.Windows.Forms.Label();
            this.pokemonListMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pokemonListView
            // 
            this.pokemonListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pokemonListView.BackColor = System.Drawing.SystemColors.Control;
            this.pokemonListView.GridLines = true;
            this.pokemonListView.Location = new System.Drawing.Point(12, 28);
            this.pokemonListView.Name = "pokemonListView";
            this.pokemonListView.Size = new System.Drawing.Size(598, 371);
            this.pokemonListView.TabIndex = 0;
            this.pokemonListView.UseCompatibleStateImageBehavior = false;
            this.pokemonListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pokemonListView_MouseClick);
            // 
            // pokemonListMenuStrip
            // 
            this.pokemonListMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortToolStripMenuItem,
            this.actionsToolStripMenuItem});
            this.pokemonListMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.pokemonListMenuStrip.Name = "pokemonListMenuStrip";
            this.pokemonListMenuStrip.Size = new System.Drawing.Size(622, 24);
            this.pokemonListMenuStrip.TabIndex = 10;
            this.pokemonListMenuStrip.Text = "PokemonForm";
            // 
            // sortToolStripMenuItem
            // 
            this.sortToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortByCPToolStripMenuItem,
            this.sortByIVToolStripMenuItem,
            this.sortByNameToolStripMenuItem});
            this.sortToolStripMenuItem.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.sortToolStripMenuItem.Name = "sortToolStripMenuItem";
            this.sortToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.sortToolStripMenuItem.Text = "Sort";
            // 
            // sortByCPToolStripMenuItem
            // 
            this.sortByCPToolStripMenuItem.Name = "sortByCPToolStripMenuItem";
            this.sortByCPToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.sortByCPToolStripMenuItem.Text = "By CP";
            this.sortByCPToolStripMenuItem.Click += new System.EventHandler(this.sortByCPToolStripMenuItem_Click);
            // 
            // sortByIVToolStripMenuItem
            // 
            this.sortByIVToolStripMenuItem.Name = "sortByIVToolStripMenuItem";
            this.sortByIVToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.sortByIVToolStripMenuItem.Text = "By IV";
            this.sortByIVToolStripMenuItem.Click += new System.EventHandler(this.sortByIVToolStripMenuItem_Click);
            // 
            // sortByNameToolStripMenuItem
            // 
            this.sortByNameToolStripMenuItem.Name = "sortByNameToolStripMenuItem";
            this.sortByNameToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.sortByNameToolStripMenuItem.Text = "By Name";
            this.sortByNameToolStripMenuItem.Click += new System.EventHandler(this.sortByNameToolStripMenuItem_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transferSelectedToolStripMenuItem,
            this.evolveSelectedToolStripMenuItem,
            this.powerupSelectedToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.actionsToolStripMenuItem.Text = "Actions";
            // 
            // transferSelectedToolStripMenuItem
            // 
            this.transferSelectedToolStripMenuItem.Name = "transferSelectedToolStripMenuItem";
            this.transferSelectedToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.transferSelectedToolStripMenuItem.Text = "Transfer selected";
            this.transferSelectedToolStripMenuItem.Click += new System.EventHandler(this.transferSelectedToolStripMenuItem_Click);
            // 
            // evolveSelectedToolStripMenuItem
            // 
            this.evolveSelectedToolStripMenuItem.Name = "evolveSelectedToolStripMenuItem";
            this.evolveSelectedToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.evolveSelectedToolStripMenuItem.Text = "Evolve selected";
            this.evolveSelectedToolStripMenuItem.Click += new System.EventHandler(this.evolveSelectedToolStripMenuItem_Click);
            // 
            // powerupSelectedToolStripMenuItem
            // 
            this.powerupSelectedToolStripMenuItem.Name = "powerupSelectedToolStripMenuItem";
            this.powerupSelectedToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.powerupSelectedToolStripMenuItem.Text = "Powerup selected";
            this.powerupSelectedToolStripMenuItem.Click += new System.EventHandler(this.powerupSelectedToolStripMenuItem_Click);
            // 
            // lbPokeListLoading
            // 
            this.lbPokeListLoading.AutoSize = true;
            this.lbPokeListLoading.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPokeListLoading.Location = new System.Drawing.Point(255, 190);
            this.lbPokeListLoading.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lbPokeListLoading.Name = "lbPokeListLoading";
            this.lbPokeListLoading.Size = new System.Drawing.Size(102, 30);
            this.lbPokeListLoading.TabIndex = 11;
            this.lbPokeListLoading.Text = "Loading...";
            // 
            // PokemonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(622, 411);
            this.Controls.Add(this.lbPokeListLoading);
            this.Controls.Add(this.pokemonListMenuStrip);
            this.Controls.Add(this.pokemonListView);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "PokemonForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "My Pokemon List";
            this.Load += new System.EventHandler(this.PokemonForm_Load);
            this.pokemonListMenuStrip.ResumeLayout(false);
            this.pokemonListMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView pokemonListView;
        private System.Windows.Forms.MenuStrip pokemonListMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem sortToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByCPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByIVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transferSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evolveSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem powerupSelectedToolStripMenuItem;
        private System.Windows.Forms.Label lbPokeListLoading;
    }
}