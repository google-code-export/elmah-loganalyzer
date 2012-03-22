﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using ElmahLogAnalyzer.Core.Domain;
using ElmahLogAnalyzer.Core.Presentation;

namespace ElmahLogAnalyzer.UI.Views.Partials
{
	public partial class SearchResultView : UserControl
	{
		public SearchResultView()
		{
			InitializeComponent();
		}

		public event EventHandler<ErrorLogSelectedEventArgs> OnErrorLogSelected;

		public void LoadTree(IList<ErrorLog> errorLogs)
		{
			ClearView();

			DisplayNumberOfErrors(errorLogs.Count);
			
			if (errorLogs.Count == 0)
			{
				CreateNoResultsNode();
				return;
			}

			foreach (var error in errorLogs)
			{
				var currentDate = error.Time.Date.ToShortDateString();
				var dateNode = GetDateFolderNode(currentDate) ?? CreateDateFolderNode(currentDate);
				CreateErrorLogNode(dateNode, error);
			}
		}
		
		public void ClearView()
		{
			if (InvokeRequired)
			{
				this.InvokeEx(x => x.Clear());
			}
			else
			{
				Clear();
			}
		}

		private static void CreateErrorLogNode(TreeNode dateFolderNode, ErrorLog errorLog)
		{
			var node = dateFolderNode.Nodes.Add(errorLog.Time.ToString());
			node.Tag = errorLog;
			node.ImageIndex = 1;
			node.SelectedImageIndex = 1;
		}

		private TreeNode GetDateFolderNode(string date)
		{
			return _resultTreeView.Nodes.ContainsKey(date) ? _resultTreeView.Nodes[date] : null;
		}

		private TreeNode CreateDateFolderNode(string date)
		{
			var node = _resultTreeView.Nodes.Add(date, date);
			node.ImageIndex = 0;
			node.SelectedImageIndex = 0;

			return node;
		}
		
		private void CreateNoResultsNode()
		{
			var node = _resultTreeView.Nodes.Add("No logs found");
			node.ImageIndex = 2;
			node.SelectedImageIndex = 2;
		}
		
		private void DisplayNumberOfErrors(int numberOfErrors)
		{
			_numberOfResultsLabel.Text = string.Format("Logs found: {0}", numberOfErrors);
		}

		private void ResultTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			var errorLog = e.Node.Tag as ErrorLog;
			if (errorLog == null)
			{
				return;
			}

			if (OnErrorLogSelected != null)
			{
				OnErrorLogSelected(this, new ErrorLogSelectedEventArgs(errorLog));
			}
		}

		private void Clear()
		{
			_resultTreeView.Nodes.Clear();
		}

		private void ResultTreeViewMouseMove(object sender, MouseEventArgs e)
		{
			var node = _resultTreeView.GetNodeAt(e.X, e.Y);
			
			if (node != null)
			{
				if (node.Tag != null)
				{
					if (node.Tag.ToString() != _toolTip.GetToolTip(_resultTreeView))
					{
						_toolTip.SetToolTip(_resultTreeView, node.Tag.ToString());
					}
				}
				else
				{
					_toolTip.SetToolTip(_resultTreeView, string.Empty);
				}
			}
			else
			{
				_toolTip.SetToolTip(_resultTreeView, string.Empty);
			}
		}
	}
}
