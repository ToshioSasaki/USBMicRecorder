using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Lame;
using System.Diagnostics;

namespace USBMicRecorder
{
    /// <summary>
    /// USB�}�C�N�̘^�����s���t�H�[���N���X�ł��B
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// �^���L���v�`�����Ǘ����邽�߂̃����o�ϐ�
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
        private bool isDisposing = false; // �t���O�ǉ�


        /// <summary>
        /// �R���X�g���N�^�ł��B
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            // �Đ��f�o�C�X�̏�����
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
            // �^���f�o�C�X�̏�����
            LoadDevices();
            tbVolume.Minimum = 10;
            tbVolume.Maximum = 115;
            tbVolume.Value = tbVolume.Maximum;
            volume = 1.0f;
            // �^���{�����[���ύX���ɍĐ��{�����[�����A��������
            tbVolume.Scroll += (s, e) =>
            {
                volume = Math.Max((float)tbVolume.Value / tbVolume.Maximum, 0.01f);

                // �Đ����ɂ����f
                trkVolume.Value = tbVolume.Value;
                if (audioFile != null)
                {
                    audioFile.Volume = trkVolume.Value / 100f;
                }
            };
            lblCapture.Visible = false;
            // �O��ۑ��p�X������΃e�L�X�g�{�b�N�X�ɃZ�b�g
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
        /// �t�H�[���̃��[�h�C�x���g�n���h���ł��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// �ۑ����I�����邽�߂̃t�H���_�u���E�U�_�C�A���O��\�����܂��B
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
        /// MP3��I�����邽�߂̃R���{�{�b�N�X�̑I��ύX�C�x���g�n���h���ł��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayDir_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {

                ofd.InitialDirectory = Path.GetDirectoryName(txtPlayPath.Text);
                ofd.Filter = "MP3�t�@�C�� (*.mp3)|*.mp3";   // MP3�t�@�C���Ɍ���
                ofd.Title = "MP3�t�@�C����I�����Ă�������";
                ofd.RestoreDirectory = true;                // �_�C�A���O�������ɃJ�����g�f�B���N�g�������ɖ߂��i�I�v�V�����j

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPlayPath.Text = ofd.FileName;
                    appSettings.LastPlayDirectory = txtPlayPath.Text;
                    SettingsManager.Save(appSettings);
                }
            }
        }

        /// <summary>
        /// �^���{�^���̃N���b�N�C�x���g�n���h���ł��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRecord_Click_1(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                if (string.IsNullOrEmpty(savePath))
                {
                    MessageBox.Show("�ۑ����I��ł�������");
                    return;
                }
                lblCapture.Visible = true;
                StartCapture(levelOnly: true, AudioMode.ProductionMode);
                btnRecord.Text = "�^����~";
                isRecording = true;
            }
            else
            {
                lblCapture.Visible = false;
                StopCapture();
                btnRecord.Text = "�^��";
                isRecording = false;
            }
        }

        /// <summary>
        /// �e�X�g�^���{�^���̃N���b�N�C�x���g�n���h���ł��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click_1(object sender, EventArgs e)
        {
            if (!isTesting)
            {
                StartCapture(levelOnly: true, AudioMode.TestMode);
                btnTest.Text = "�e�X�g��~";
                isTesting = true;
            }
            else
            {
                StopCapture();
                btnTest.Text = "�e�X�g";
                isTesting = false;
            }
        }

        /// <summary>
        /// �ۑ��p�X�̃e�L�X�g���ύX���ꂽ�Ƃ��̃C�x���g�n���h���ł��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSavePath_TextChanged(object sender, EventArgs e)
        {
            savePath = txtSavePath.Text;
        }

        /// <summary>
        /// �Đ��{�^���̃N���b�N�C�x���g�n���h���ł��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayMp3(AudioMode.PlayMode);
        }

        /// <summary>
        /// �Đ���~�{�^���̃N���b�N�C�x���g�n���h���ł��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            StopMp3();
        }

        /// <summary>
        /// �t�H�[���̃��T�C�Y�C�x���g�n���h���ł��B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen; // �t�H�[���̈ʒu�𒆉��ɐݒ�
        }

        /// <summary>
        /// �Đ���~�{�^���̃N���b�N�C�x���g�n���h���ł��B
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
        /// MP3�t�@�C�����Đ����܂��B
        /// </summary>
        private void PlayMp3(AudioMode mode)
        {
            if (!File.Exists(txtPlayPath.Text))
            {
                MessageBox.Show("�Đ��t�@�C����������܂���B");
                return;
            }

            try
            {
                playPath = txtPlayPath.Text;
                EnabledControls(enabled: false, mode); // �Đ�����UI�𖳌���

                // �Đ��t�@�C���ǂݍ���
                using (var reader = new MediaFoundationReader(playPath)) // IDE0017: �I�u�W�F�N�g�̏��������ȑf��
                {
                    audioFile = new AudioFileReader(playPath)
                    {
                        Volume = trkVolume.Value / 100f // ���ʂ��������itrkVolume�̈ʒu�ɍ��킹�Đݒ�j
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
                MessageBox.Show("�Đ��G���[: " + ex.Message);
                UnlockConrols();
            }
        }

        /// <summary>
        /// MP3�̍Đ����~���܂��B
        /// </summary>
        private void StopMp3()
        {
            try
            {
                if (outputDevice != null)
                {
                    // �C�x���g���O�����Ƃő��d�Ăяo����h�~
                    outputDevice.PlaybackStopped -= OnPlaybackStopped;

                    // �Đ����~
                    outputDevice.Stop();

                    // ���\�[�X���
                    outputDevice.Dispose();
                    outputDevice = null;
                }

                if (audioFile != null)
                {
                    audioFile.Dispose();
                    audioFile = null;
                }

                // UI �����ɖ߂�
                UnlockConrols();
            }
            catch (Exception ex)
            {
                MessageBox.Show("�Đ���~�G���[: " + ex.Message);
            }
        }


        /// <summary>
        /// �^��UI�̗L��/������؂�ւ��܂��B
        /// </summary>
        /// <param name="enabled">True:�L���AFalse�F����</param>
        /// <param name="AudioClientShareMode">�I�[�f�B�I�N���C�A���g�̋��L���[�h</param>
        private void EnabledControls(bool enabled, AudioMode mode)
        {
            // �^���J�n�O��UI�X�V
            tbVolume.Enabled = enabled;       // �^�����̓{�����[�������s��
            cbDevices.Enabled = enabled;      // �^�����̓f�o�C�X�ύX�s��
            btnBrowse.Enabled = enabled;      // �^�����͕ۑ���ύX�s��
            txtSavePath.Enabled = enabled;    // �^�����͕ۑ���ύX�s��
            if (AudioMode.PlayMode != mode)
            {
                // �Đ��ȊO�̓e�X�g�{�^���E�^���{�^����UI�𖳌���
                btnTest.Enabled = mode == AudioMode.TestMode;
                btnRecord.Enabled = mode == AudioMode.ProductionMode;
            }
            // �Đ��֘A��UI�X�V
            cbPlayDevice.Enabled = enabled;    //�Đ��͖�����
            txtPlayPath.Enabled = enabled;     //�Đ��̓p�X�ύX�s��
            btnPlayDir.Enabled = enabled;      //�Đ��̓{�^�������s��
        }

        /// <summary>
        /// �^��/�Đ��R���g���[����Enable=true(�A�����b�N)���܂��B
        /// </summary>
        private void UnlockConrols()
        {
            tbVolume.Enabled = true;       // �{�����[���L����
            cbDevices.Enabled = true;      // �f�o�C�X�L����
            btnBrowse.Enabled = true;      // �ۑ���{�^���L����
            txtSavePath.Enabled = true;    // �ۑ���L����
            btnTest.Enabled = true;        // �e�X�g�^���{�^����L���� 
            btnRecord.Enabled = true;      // �^���{�^����L����
            cbPlayDevice.Enabled = true;   // �Đ��͗L����
            txtPlayPath.Enabled = true;    // �Đ��̓p�X�L����
            btnPlayDir.Enabled = true;     // �Đ��̓{�^���L����
        }

        /// <summary>
        /// �I�[�f�B�I�N���C�A���g�̋��L���[�h���`���܂��B
        /// </summary>
        private enum AudioMode
        {
            TestMode,           // �e�X�g�^�����[�h
            PlayMode,           // �Đ����[�h
            ProductionMode,     // �{�Ԙ^�����[�h
        }

        /// <summary>
        /// �^�����J�n���܂��B
        /// </summary>
        /// <param name="levelOnly">�^�����x��</param>
        // --- StartCapture ---
        private void StartCapture(bool levelOnly, AudioMode mode)
        {
            // �����̃L���v�`��������Β�~���ĉ��
            var enumerator = new MMDeviceEnumerator();
            var dev = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)[cbDevices.SelectedIndex];
            capture = new WasapiCapture(dev);

            // �{�����[���w�肪�����MP3�̏������݂��s��������
            if (levelOnly)
            {
                saveFilePath = Path.Combine(txtSavePath.Text, $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.mp3");
                capture.WaveFormat = new WaveFormat(44100, 16, 2); // 44.1kHz, 16bit, Stereo
                mp3Writer = new LameMP3FileWriter(saveFilePath, capture.WaveFormat, LAMEPreset.ABR_128);
            }

            // UI�R���g���[���𖳌���
            EnabledControls(enabled: false, mode);

            // �^���J�n
            capture.DataAvailable += (s, e) =>
            {

                double sumSquares = 0;
                int sampleCount = e.BytesRecorded / 2;
                byte[]? outBuffer = null;

                // ���x���̂ݎ擾�̏ꍇ�͏o�̓o�b�t�@��������
                if (levelOnly && mp3Writer != null)
                {
                    outBuffer = new byte[e.BytesRecorded];
                }

                // �̐ς�0.0�`1.0�͈̔͂ɐ���
                float gain = volume; // 1.0�܂�

                // 2�o�C�g���ƂɃT���v�����擾���ARMS�v�Z
                for (int i = 0; i < e.BytesRecorded; i += 2)
                {
                    short sample = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]);
                    float sample32 = sample / 32768f;
                    float adjusted = sample32 * gain;

                    // ���S�K�[�h
                    if (float.IsNaN(adjusted) || float.IsInfinity(adjusted))
                    {
                        adjusted = 0;
                    }

                    // �N���b�s���O�i�قڕs�v�ɂȂ邪�O�̂��߁j
                    if (adjusted > 0.98f) adjusted = 0.98f;
                    if (adjusted < -0.98f) adjusted = -0.98f;

                    sumSquares += adjusted * adjusted;

                    // �o�̓o�b�t�@�ɏ������݂̏���
                    if (levelOnly && mp3Writer != null && outBuffer != null && (i + 1) < outBuffer.Length)
                    {
                        short outSample = (short)(adjusted * 32767);
                        outBuffer[i] = (byte)(outSample & 0xFF);
                        outBuffer[i + 1] = (byte)((outSample >> 8) & 0xFF);
                    }
                }

                // ���[�^�[�\��
                double rms = Math.Sqrt(sumSquares / sampleCount);
                int dB = (rms > 0) ? (int)(20.0 * Math.Log10(rms)) : -60;
                dB = Math.Max(dB, -60);
                int meterValue = dB + 60;
                meterValue = Math.Min(Math.Max(meterValue, pbLevel.Minimum), pbLevel.Maximum);

                // UI�X���b�h�Ńv���O���X�o�[���X�V
                pbLevel.Invoke((Action)(() =>
                {
                    pbLevel.Value = meterValue;
                }));

                // MP3��������
                if (levelOnly && outBuffer != null && outBuffer.Length > 0)
                {
                    if (isDisposing)
                    {
                        // �^����~���͐�΂�Write���Ă΂Ȃ�
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
                            // MP3�t�@�C���ɏ�������
                            writerSnapshot.Write(outBuffer, 0, outBuffer.Length);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"MP3 write error: {ex.Message}");
                        }
                    }
                }
            };

            // �^����~�C�x���g�n���h��
            capture.RecordingStopped += (s, e) =>
            {
                lock (mp3WriterLock)
                {
                    mp3Writer?.Dispose();
                    mp3Writer = null;
                }
                pbLevel.Value = 0;
                // UI�R���g���[����L����
                UnlockConrols();
            };

            pbLevel.Maximum = 60;
            pbLevel.Minimum = 0;

            capture.StartRecording();
        }



        /// <summary>
        /// �^�����~���A���\�[�X��������܂��B
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
        /// �f�o�C�X��ǂݍ��݁A�R���{�{�b�N�X�ɒǉ����܂��B
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
        /// �Đ��f�o�C�X��ǂݍ��݁A�R���{�{�b�N�X�ɒǉ����܂��B
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
