﻿using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Crepido.ElmahOfflineViewer.Core.Common;
using Crepido.ElmahOfflineViewer.Core.Domain;
using Crepido.ElmahOfflineViewer.Core.Infrastructure.Dependencies;
using Crepido.ElmahOfflineViewer.Core.Presentation;
using Crepido.ElmahOfflineViewer.UI.Views;

namespace Crepido.ElmahOfflineViewer.UI.Forms
{
	public partial class MainForm : Form
	{
		private readonly IErrorLogRepository _repository;

		public MainForm()
		{
			InitializeComponent();

			DisplayApplicationVersion();

			_repository = ServiceLocator.Resolve<IErrorLogRepository>();
			_repository.OnInitialized += RepositoryOnInitialized;

		    var directory = Environment.GetCommandLineArgs()
                                        .Skip(1)
                                        .FirstOrDefault(arg => arg.HasValue());

            if (!directory.HasValue())
            {
                directory = ConfigurationManager.AppSettings["LogsDirectory"];

                if (!directory.HasValue())
                {
                    SetErrorLoadingState();
                    return;
                }
            }

		    HandleLoadingFromDirectory(directory);
		}
	
		private void SetLoadingState()
		{
			_showSearchViewButton.Enabled = false;
			_showReportViewButton.Enabled = false;
			_selectDirectoryButton.Enabled = false;
			_selectServerButton.Enabled = false;
			_showSettingsViewButton.Enabled = false;

			LoadView(new LoadingView());
		}

		private void SetReadyForWorkState()
		{
			_showSearchViewButton.Enabled = true;
			_showReportViewButton.Enabled = true;
			_selectDirectoryButton.Enabled = true;
			_selectServerButton.Enabled = true;
			_showSettingsViewButton.Enabled = true;

			_mainPanel.Controls.Clear();
		}
		
		private void SetErrorLoadingState()
		{
			_showSearchViewButton.Enabled = false;
			_showReportViewButton.Enabled = false;
			_selectDirectoryButton.Enabled = true;
			_selectServerButton.Enabled = true;
			_showSettingsViewButton.Enabled = true;
			
			_mainPanel.Controls.Clear();
			directoryToolStripStatusLabel.Text = string.Empty;
		}

		private void HandleDownloadingLogs(string url)
		{
			var downloader = ServiceLocator.Resolve<IErrorLogDownloader>();
			downloader.Download(new Uri(url));
			HandleLoadingFromDirectory(downloader.DownloadDirectory);
		}
		
		private void HandleLoadingFromDirectory(string directory)
		{
			SetLoadingState();
			directoryToolStripStatusLabel.Text = string.Format("Loading logs from: {0}", directory);

			var thread = new Thread(InitializeRepository);
			thread.Start(directory);
		}

		private void InitializeRepository(object directory)
		{
			try
			{
				_repository.Initialize(directory as string);

				if (InvokeRequired)
				{
					this.InvokeEx(x => x.SetReadyForWorkState());
				}
				else
				{
					SetReadyForWorkState();
				}
			}
			catch (Exception ex)
			{
				if (InvokeRequired)
				{
					this.InvokeEx(x => x.DisplayError(ex));
					this.InvokeEx(x => x.SetErrorLoadingState());
				}
				else
				{
					DisplayError(ex);
					SetErrorLoadingState();
				}
			}
		}

		private void LoadView(Control view)
		{
			_mainPanel.Controls.Clear();
			_mainPanel.Controls.Add(view);
			view.Dock = DockStyle.Fill;
		}
		
		private void DisplayError(object ex)
		{
			var error = ex as Exception;
			
			if (error == null)
			{
				return;
			}

			MessageBox.Show(this, error.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void DisplayApplicationVersion()
		{
			versionStripStatusLabel.Text = string.Format("Build: {0} ({1})", Application.ProductVersion, GetType().Assembly.GetTypeOfBuild());
		}
		
		private void SelectDirectoryButtonClick(object sender, EventArgs e)
		{
			_folderBrowserDialog.SelectedPath = _repository.Directory;

			var result = _folderBrowserDialog.ShowDialog(this);

			if (result != DialogResult.OK)
			{
				return;
			}

			HandleLoadingFromDirectory(_folderBrowserDialog.SelectedPath);
		}
		
		private void ShowSearchViewButtonClick(object sender, EventArgs e)
		{
			var presenter = ServiceLocator.Resolve<SearchPresenter>();
			LoadView(presenter.View as Control);
		}

		private void ShowReportViewButtonClick(object sender, EventArgs e)
		{
			var presenter = ServiceLocator.Resolve<ReportPresenter>();
			LoadView(presenter.View as Control);
		}

		private void ShowSettingsViewButtonClick(object sender, EventArgs e)
		{
			var presenter = ServiceLocator.Resolve<SettingsPresenter>();
			var view = presenter.View as Form;
			view.ShowDialog(this);
		}

		private void ShowAboutButtonClick(object sender, EventArgs e)
		{
			var view = new AboutForm();
			view.ShowDialog(this);
		}

		private void SelectServerButtonClick(object sender, EventArgs e)
		{
			var presenter = ServiceLocator.Resolve<SelectServerPresenter>();
			var view = presenter.View as Form;
			var dialogResult = view.ShowDialog(this);

			if (dialogResult == DialogResult.OK)
			{
				var url = ((ISelectServerView)view).Url;
				HandleDownloadingLogs(url);
			}
		}
	}
}
