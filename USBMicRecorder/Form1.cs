using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Lame;
using System.Diagnostics;

namespace USBMicRecorder
{
    /// <summary>
    /// USBマイクの録音を行うフォームクラスです。
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// 録音キャプチャを管理するためのメンバ変数
        /// </summary>
        private WasapiCapture? capture;
        private IWavePlayer? outputDevice;
        private AudioFileReader? audioFile;
        private LameMP3FileWriter? mp3Writer;
        private AppSettings appSettings;
        private string savePath = "";
        private string playPath = "";
        private string saveFilePath = "";
        private volatile float volume = 1.0f; // 0.0~1.0
        private bool isTesting = false;
        private bool isRecording = false;
        private readonly object mp3WriterLock = new object();
        private bool isDisposing = false; // フラグ追加


        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            // 再生デバイスの初期化
            LoadPlayDevices();
            trkVolume.Minimum = 0;
            trkVolume.Maximum = 100;
            trkVolume.Value = 50;
            trkVolume.Scroll += (s, e) =>
            {
                if (audioFile != null)
                    audioFile.Volume = trkVolume.Value / 100f;
            };
            lblPlay.Visible = false;
            // 録音デバイスの初期化
            LoadDevices();
            tbVolume.Minimum = 10;
            tbVolume.Maximum = 115;
            tbVolume.Value = tbVolume.Maximum;
            volume = 1.0f;
            // 録音ボリューム変更時に再生ボリュームも連動させる
            tbVolume.Scroll += (s, e) =>
            {
                volume = Math.Max((float)tbVolume.Value / tbVolume.Maximum, 0.01f);

                // 再生側にも反映
                trkVolume.Value = tbVolume.Value;
                if (audioFile != null)
                {
                    audioFile.Volume = trkVolume.Value / 100f;
                }
            };
            lblCapture.Visible = false;
            // 前回保存パスがあればテキストボックスにセット
            appSettings = SettingsManager.Load();
            if (!string.IsNullOrEmpty(appSettings.LastSaveDirectory))
            {
                txtSavePath.Text = appSettings.LastSaveDirectory;
                savePath = appSettings.LastSaveDirectory;
            }
            if (!string.IsNullOrEmpty(appSettings.LastPlayDirectory))
            {
                txtPlayPath.Text = appSettings.LastPlayDirectory;
                playPath = appSettings.LastPlayDirectory;
            }
        }

        /// <summary>
        /// フォームのロードイベントハンドラです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 保存先を選択するためのフォルダブラウザダイアログを表示します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click_1(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.InitialDirectory = appSettings.LastSaveDirectory;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtSavePath.Text = fbd.SelectedPath;
                    appSettings.LastSaveDirectory = fbd.SelectedPath;
                    SettingsManager.Save(appSettings);
                }
            }
        }

        /// <summary>
        /// MP3を選択するためのコンボボックスの選択変更イベントハンドラです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayDir_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {

                ofd.InitialDirectory = Path.GetDirectoryName(txtPlayPath.Text);
                ofd.Filter = "MP3ファイル (*.mp3)|*.mp3";   // MP3ファイルに限定
                ofd.Title = "MP3ファイルを選択してください";
                ofd.RestoreDirectory = true;                // ダイアログを閉じた後にカレントディレクトリを元に戻す（オプション）

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPlayPath.Text = ofd.FileName;
                    appSettings.LastPlayDirectory = txtPlayPath.Text;
                    SettingsManager.Save(appSettings);
                }
            }
        }

        /// <summary>
        /// 録音ボタンのクリックイベントハンドラです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRecord_Click_1(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                if (string.IsNullOrEmpty(savePath))
                {
                    MessageBox.Show("保存先を選んでください");
                    return;
                }
                lblCapture.Visible = true;
                StartCapture(levelOnly: true, AudioMode.ProductionMode);
                btnRecord.Text = "録音停止";
                isRecording = true;
            }
            else
            {
                lblCapture.Visible = false;
                StopCapture();
                btnRecord.Text = "録音";
                isRecording = false;
            }
        }

        /// <summary>
        /// テスト録音ボタンのクリックイベントハンドラです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click_1(object sender, EventArgs e)
        {
            if (!isTesting)
            {
                StartCapture(levelOnly: true, AudioMode.TestMode);
                btnTest.Text = "テスト停止";
                isTesting = true;
            }
            else
            {
                StopCapture();
                btnTest.Text = "テスト";
                isTesting = false;
            }
        }

        /// <summary>
        /// 保存パスのテキストが変更されたときのイベントハンドラです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSavePath_TextChanged(object sender, EventArgs e)
        {
            savePath = txtSavePath.Text;
        }

        /// <summary>
        /// 再生ボタンのクリックイベントハンドラです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayMp3(AudioMode.PlayMode);
        }

        /// <summary>
        /// 再生停止ボタンのクリックイベントハンドラです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            StopMp3();
        }

        /// <summary>
        /// フォームのリサイズイベントハンドラです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen; // フォームの位置を中央に設定
        }

        /// <summary>
        /// 再生停止ボタンのクリックイベントハンドラです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            outputDevice?.Dispose();
            outputDevice = null;

            audioFile?.Dispose();
            audioFile = null;

            UnlockConrols();
        }

        /// <summary>
        /// MP3ファイルを再生します。
        /// </summary>
        private void PlayMp3(AudioMode mode)
        {
            if (!File.Exists(txtPlayPath.Text))
            {
                MessageBox.Show("再生ファイルが見つかりません。");
                return;
            }

            try
            {
                playPath = txtPlayPath.Text;
                EnabledControls(enabled: false, mode); // 再生中はUIを無効化

                // 再生ファイル読み込み
                using (var reader = new MediaFoundationReader(playPath)) // IDE0017: オブジェクトの初期化を簡素化
                {
                    audioFile = new AudioFileReader(playPath)
                    {
                        Volume = trkVolume.Value / 100f // 音量を初期化（trkVolumeの位置に合わせて設定）
                    };
                }

                var enumerator = new MMDeviceEnumerator();
                var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                var selectedDevice = devices[cbPlayDevice.SelectedIndex];

                outputDevice = new WasapiOut(selectedDevice, AudioClientShareMode.Shared, true, 100);
                outputDevice.Init(audioFile);
                outputDevice.PlaybackStopped += OnPlaybackStopped;
                outputDevice.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("再生エラー: " + ex.Message);
                UnlockConrols();
            }
        }

        /// <summary>
        /// MP3の再生を停止します。
        /// </summary>
        private void StopMp3()
        {
            try
            {
                if (outputDevice != null)
                {
                    // イベントを外すことで多重呼び出しを防止
                    outputDevice.PlaybackStopped -= OnPlaybackStopped;

                    // 再生を停止
                    outputDevice.Stop();

                    // リソース解放
                    outputDevice.Dispose();
                    outputDevice = null;
                }

                if (audioFile != null)
                {
                    audioFile.Dispose();
                    audioFile = null;
                }

                // UI を元に戻す
                UnlockConrols();
            }
            catch (Exception ex)
            {
                MessageBox.Show("再生停止エラー: " + ex.Message);
            }
        }


        /// <summary>
        /// 録音UIの有効/無効を切り替えます。
        /// </summary>
        /// <param name="enabled">True:有効、False：無効</param>
        /// <param name="AudioClientShareMode">オーディオクライアントの共有モード</param>
        private void EnabledControls(bool enabled, AudioMode mode)
        {
            // 録音開始前のUI更新
            tbVolume.Enabled = enabled;       // 録音中はボリューム調整不可
            cbDevices.Enabled = enabled;      // 録音中はデバイス変更不可
            btnBrowse.Enabled = enabled;      // 録音中は保存先変更不可
            txtSavePath.Enabled = enabled;    // 録音中は保存先変更不可
            if (AudioMode.PlayMode != mode)
            {
                // 再生以外はテストボタン・録音ボタンのUIを無効化
                btnTest.Enabled = mode == AudioMode.TestMode;
                btnRecord.Enabled = mode == AudioMode.ProductionMode;
            }
            // 再生関連のUI更新
            cbPlayDevice.Enabled = enabled;    //再生は無効化
            txtPlayPath.Enabled = enabled;     //再生はパス変更不可
            btnPlayDir.Enabled = enabled;      //再生はボタン押下不可
        }

        /// <summary>
        /// 録音/再生コントロールをEnable=true(アンロック)します。
        /// </summary>
        private void UnlockConrols()
        {
            tbVolume.Enabled = true;       // ボリューム有効化
            cbDevices.Enabled = true;      // デバイス有効化
            btnBrowse.Enabled = true;      // 保存先ボタン有効化
            txtSavePath.Enabled = true;    // 保存先有効化
            btnTest.Enabled = true;        // テスト録音ボタンを有効化 
            btnRecord.Enabled = true;      // 録音ボタンを有効化
            cbPlayDevice.Enabled = true;   // 再生は有効化
            txtPlayPath.Enabled = true;    // 再生はパス有効化
            btnPlayDir.Enabled = true;     // 再生はボタン有効化
        }

        /// <summary>
        /// オーディオクライアントの共有モードを定義します。
        /// </summary>
        private enum AudioMode
        {
            TestMode,           // テスト録音モード
            PlayMode,           // 再生モード
            ProductionMode,     // 本番録音モード
        }

        /// <summary>
        /// 録音を開始します。
        /// </summary>
        /// <param name="levelOnly">録音レベル</param>
        // --- StartCapture ---
        private void StartCapture(bool levelOnly, AudioMode mode)
        {
            // 既存のキャプチャがあれば停止して解放
            var enumerator = new MMDeviceEnumerator();
            var dev = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)[cbDevices.SelectedIndex];
            capture = new WasapiCapture(dev);

            // ボリューム指定があればMP3の書き込みを行う初期化
            if (levelOnly)
            {
                saveFilePath = Path.Combine(txtSavePath.Text, $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.mp3");
                capture.WaveFormat = new WaveFormat(44100, 16, 2); // 44.1kHz, 16bit, Stereo
                mp3Writer = new LameMP3FileWriter(saveFilePath, capture.WaveFormat, LAMEPreset.ABR_128);
            }

            // UIコントロールを無効化
            EnabledControls(enabled: false, mode);

            // 録音開始
            capture.DataAvailable += (s, e) =>
            {

                double sumSquares = 0;
                int sampleCount = e.BytesRecorded / 2;
                byte[]? outBuffer = null;

                // レベルのみ取得の場合は出力バッファを初期化
                if (levelOnly && mp3Writer != null)
                {
                    outBuffer = new byte[e.BytesRecorded];
                }

                // 体積を0.0～1.0の範囲に制限
                float gain = volume; // 1.0まで

                // 2バイトごとにサンプルを取得し、RMS計算
                for (int i = 0; i < e.BytesRecorded; i += 2)
                {
                    short sample = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]);
                    float sample32 = sample / 32768f;
                    float adjusted = sample32 * gain;

                    // 安全ガード
                    if (float.IsNaN(adjusted) || float.IsInfinity(adjusted))
                    {
                        adjusted = 0;
                    }

                    // クリッピング（ほぼ不要になるが念のため）
                    if (adjusted > 0.98f) adjusted = 0.98f;
                    if (adjusted < -0.98f) adjusted = -0.98f;

                    sumSquares += adjusted * adjusted;

                    // 出力バッファに書き込みの準備
                    if (levelOnly && mp3Writer != null && outBuffer != null && (i + 1) < outBuffer.Length)
                    {
                        short outSample = (short)(adjusted * 32767);
                        outBuffer[i] = (byte)(outSample & 0xFF);
                        outBuffer[i + 1] = (byte)((outSample >> 8) & 0xFF);
                    }
                }

                // メーター表示
                double rms = Math.Sqrt(sumSquares / sampleCount);
                int dB = (rms > 0) ? (int)(20.0 * Math.Log10(rms)) : -60;
                dB = Math.Max(dB, -60);
                int meterValue = dB + 60;
                meterValue = Math.Min(Math.Max(meterValue, pbLevel.Minimum), pbLevel.Maximum);

                // UIスレッドでプログレスバーを更新
                pbLevel.Invoke((Action)(() =>
                {
                    pbLevel.Value = meterValue;
                }));

                // MP3書き込み
                if (levelOnly && outBuffer != null && outBuffer.Length > 0)
                {
                    if (isDisposing)
                    {
                        // 録音停止中は絶対にWriteを呼ばない
                        return;
                    }
                    LameMP3FileWriter? writerSnapshot = null;
                    lock (mp3WriterLock)
                    {
                        writerSnapshot = mp3Writer;
                    }
                    if (writerSnapshot != null)
                    {
                        try
                        {
                            // MP3ファイルに書き込み
                            writerSnapshot.Write(outBuffer, 0, outBuffer.Length);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"MP3 write error: {ex.Message}");
                        }
                    }
                }
            };

            // 録音停止イベントハンドラ
            capture.RecordingStopped += (s, e) =>
            {
                lock (mp3WriterLock)
                {
                    mp3Writer?.Dispose();
                    mp3Writer = null;
                }
                pbLevel.Value = 0;
                // UIコントロールを有効化
                UnlockConrols();
            };

            pbLevel.Maximum = 60;
            pbLevel.Minimum = 0;

            capture.StartRecording();
        }



        /// <summary>
        /// 録音を停止し、リソースを解放します。
        /// </summary>
        private void StopCapture()
        {
            if (capture != null)
            {
                capture.StopRecording();
                capture.Dispose();
                capture = null;
            }
        }

        /// <summary>
        /// デバイスを読み込み、コンボボックスに追加します。
        /// </summary>
        private void LoadDevices()
        {
            cbDevices.Items.Clear();
            var enumerator = new MMDeviceEnumerator();
            foreach (var dev in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                cbDevices.Items.Add(dev.FriendlyName);
                if (cbDevices.Items.Count > 0)
                {
                    cbDevices.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// 再生デバイスを読み込み、コンボボックスに追加します。
        /// </summary>
        private void LoadPlayDevices()
        {
            cbPlayDevice.Items.Clear();
            var enumerator = new MMDeviceEnumerator();
            foreach (var dev in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                cbPlayDevice.Items.Add(dev.FriendlyName);
            }

            if (cbPlayDevice.Items.Count > 0)
            {
                cbPlayDevice.SelectedIndex = 0;
            }
        }


    }
}
