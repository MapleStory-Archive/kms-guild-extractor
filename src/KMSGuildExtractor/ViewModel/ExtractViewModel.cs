using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;

using Microsoft.Win32;

namespace KMSGuildExtractor.ViewModel
{
    public class ExtractViewModel : BindableBase
    {
        private enum State
        {
            Ready, GettingMemberList, GettingMemberdata, Done, Error
        }

        public Visibility LoadingVisibility
        {
            get => _loadingVisibility;
            set => SetProperty(ref _loadingVisibility, value, nameof(LoadingVisibility));
        }

        public Visibility DoneVisibility
        {
            get => _doneVisibility;
            set => SetProperty(ref _doneVisibility, value, nameof(DoneVisibility));
        }

        public Visibility ErrorVisibility
        {
            get => _errorVisibility;
            set => SetProperty(ref _errorVisibility, value, nameof(ErrorVisibility));
        }

        public string StateMessage
        {
            get => _stateMessage;
            set => SetProperty(ref _stateMessage, value, nameof(StateMessage));
        }

        public Brush StateColor
        {
            get => _stateColor;
            set => SetProperty(ref _stateColor, value, nameof(StateColor));
        }

        public bool CanExtract
        {
            get => _canExtract;
            set => SetProperty(ref _canExtract, value, nameof(CanExtract));
        }

        public DelegateCommand ExtractCommand { get; }

        private readonly Guild _guild;

        private Visibility _loadingVisibility;
        private Visibility _errorVisibility;
        private Visibility _doneVisibility;
        private bool _canExtract;
        private string _stateMessage = string.Empty;
        private Brush _stateColor = Brushes.Transparent;

        public ExtractViewModel(Guild guildData)
        {
            _guild = guildData;
            CanExtract = false;
            SetState(State.Ready);
            StateMessage = string.Empty;
            ExtractCommand = new DelegateCommand(ExecuteExtractCommand);
            Task.Run(LoadData);
        }

        private async Task LoadData()
        {
            int max;
            int count = 0;
            int errorCount = 0;

            ConcurrentQueue<int> memberIndex;

            try
            {
                StateMessage = LocalizationString.state_get_members;
                SetState(State.GettingMemberList);

                await _guild.LoadGuildMembersAsync();
                max = _guild.Members.Count;
                memberIndex = new ConcurrentQueue<int>(Enumerable.Range(0, _guild.Members.Count));

                StateMessage = string.Format(LocalizationString.state_get_data, max, 0);
                SetState(State.GettingMemberdata);

                Task loadTask1 = Load();
                Task loadTask2 = Load();
                Task loadTask3 = Load();

                await loadTask1;
                await loadTask2;
                await loadTask3;
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception)
            {
                errorCount++;
            }
            finally
            {
                if (errorCount == 0)
                {
                    StateMessage = LocalizationString.state_done;
                    SetState(State.Done);

                    CanExtract = true;
                }
                else
                {
                    StateMessage = $"Error Count: {errorCount}";
                    SetState(State.Error);
                    CanExtract = false;
                }
            }

            async Task Load()
            {
                while (memberIndex.TryDequeue(out int idx))
                {
                    try
                    {
                        await _guild.Members[idx].data.RequestSyncAsync(new CancellationTokenSource(TimeSpan.FromSeconds(60)).Token);
                        await _guild.Members[idx].data.LoadUserDetailAsync();
                    }
                    catch (TaskCanceledException)
                    {
                        errorCount++;
                    }
                    catch (UserSyncException)
                    {
                    }
                    catch (UserNotFoundException)
                    {
                    }
                    catch (Exception) // TODO: UserNotFoundException 예외 거르기
                    {
                        errorCount++;
                    }
                    finally
                    {
                        count++;
                        StateMessage = string.Format(LocalizationString.state_get_data, max, count);
                        await Task.Delay(2000);
                    }
                }
            }
        }

        private async void ExecuteExtractCommand(object _)
        {
            CanExtract = false;

            try
            {
                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (dialog.ShowDialog() ?? false)
                {
                    await DataExtract.CreateCSVAsync(dialog.FileName, _guild);
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                CanExtract = true;
            }
        }

        private void SetState(State state)
        {
            StateColor = GetStateColor(state);
            switch (state)
            {
                case State.Ready:
                    ErrorVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Collapsed;
                    DoneVisibility = Visibility.Visible;
                    break;
                case State.GettingMemberList:
                    DoneVisibility = Visibility.Collapsed;
                    ErrorVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Visible;
                    break;
                case State.GettingMemberdata:
                    DoneVisibility = Visibility.Collapsed;
                    ErrorVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Visible;
                    break;
                case State.Done:
                    ErrorVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Collapsed;
                    DoneVisibility = Visibility.Visible;
                    break;
                case State.Error:
                    DoneVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Collapsed;
                    ErrorVisibility = Visibility.Visible;
                    break;
            }
        }

        private Brush GetStateColor(State state) => state switch
        {
            State.Ready => Brushes.Gray,

            State.GettingMemberList => Brushes.Orange,

            State.GettingMemberdata => Brushes.LightSkyBlue,

            State.Done => Brushes.LightGreen,

            State.Error => Brushes.Red,

            _ => Brushes.Transparent,
        };
    }
}
