using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PhotoVideoPanelController : MonoBehaviour
{
    [SerializeField]
    private GameObject PreviousMediaFileButton;
    [SerializeField]
    private GameObject NextMediaFileButton;
    [SerializeField]
    private LoadImageToQuad ImageLoader;
    [SerializeField]
    private LoadVideoToQuad VideoLoader;
    [SerializeField]
    private TextMeshPro MediaFileCounterText;
    [SerializeField]
    private TextMeshPro VideoTimerText;
    [SerializeField]
    private GameObject TakePhotoButton;
    [SerializeField]
    private GameObject TakeVideoButton;
    [SerializeField]
    private GameObject DeleteButton;
    [SerializeField]
    private ObservableCollection<MediaFile> _mediaFiles;

    public UnityEvent OnNewData;

    


    private int _currentMediaIndex;
    //private List<MediaFile> _mediaFiles;
    private MenuMode _mode;

    /// <summary>
    /// resets the PhotoVideoPanel
    /// </summary>
    /// <param name="newMediaFiles">List of media files to display</param>
    public void Reset(ObservableCollection<MediaFile> newMediaFiles)
    {
        _currentMediaIndex = 0;
        _mediaFiles = newMediaFiles;
        PreviousMediaFileButton.gameObject.SetActive(false);
        //turn next button only on when there is more than 1 mediafile
        NextMediaFileButton.gameObject.SetActive(_mediaFiles.Count > 1 ? true : false);
        SetMediaFileCounter(_currentMediaIndex + 1, _mediaFiles.Count);
        VideoTimerText.text = string.Empty;

        if (_mediaFiles.Count > 0) // there are media files
        {
            LoadMediaFile(_mediaFiles[0]);

            if(_mode == MenuMode.Record)
            {
                DeleteButton.SetActive(true);
            }
        }
        else // there are no media files
        {
            ImageLoader.LoadImageToQuadByFileName(); //load default (placeholder) image
            DeleteButton.SetActive(false);
        }
    }

    /// <summary>
    /// set the mode of the PhotoVideoPanel
    /// </summary>
    /// <param name="mode"></param>
    public void SetMode(MenuMode mode)
    {
        _mode = mode;

        //deactivate buttons on replay mode
        TakePhotoButton.SetActive(mode == MenuMode.Replay ? false : true);
        TakeVideoButton.SetActive(mode == MenuMode.Replay ? false : true);
        DeleteButton.SetActive(mode == MenuMode.Replay ? false : true);
    }

    public void LoadNextMediaFile()
    {
        if (_mediaFiles != null)
        {
            // check if there is a next media file
            if (_mediaFiles.Count > 1 && _currentMediaIndex + 1 < _mediaFiles.Count)
            {
                _currentMediaIndex++;
                LoadMediaFile(_mediaFiles[_currentMediaIndex]);

                PreviousMediaFileButton.gameObject.SetActive(true);

               
                if (_currentMediaIndex + 1 >= _mediaFiles.Count)
                {
                    NextMediaFileButton.gameObject.SetActive(false);
                }

                SetMediaFileCounter(_currentMediaIndex + 1, _mediaFiles.Count);
            }
        }
    }

    private void LoadMediaFile(MediaFile mediaFile)
    {
        if (mediaFile.Type == MediaType.Image)
        {
            ImageLoader.LoadImageToQuadByFileName(mediaFile.FileName);
            VideoTimerText.text = string.Empty;
        }
        else
        {
            VideoLoader.LoadVideoToQuadByFileName(mediaFile.FileName);
        }
    }

    public void LoadPreivousMediaFile()
    {
        //check if there is a previous media file
        if (_currentMediaIndex - 1 >= 0)
        {
            _currentMediaIndex--;
            LoadMediaFile(_mediaFiles[_currentMediaIndex]);

            //check if previous media file is first. if it is, disable the previous button
            if (_currentMediaIndex <= 0)
            {
                PreviousMediaFileButton.gameObject.SetActive(false);
            }

            if (_currentMediaIndex + 1 < _mediaFiles.Count)
            {
                NextMediaFileButton.gameObject.SetActive(true);
            }
            SetMediaFileCounter(_currentMediaIndex + 1,  _mediaFiles.Count);
        }
    }

    public void TookPhoto(string fileName)
    {
        if (fileName != "Error")
        {
            _mediaFiles.Add(new MediaFile(fileName, MediaType.Image));
            SetUIForLatestMediaFile();
            DeleteButton.SetActive(true);
            NewData();
        }
    }

    public void TookVideo(string fileName)
    {
        if (fileName != "Error")
        {
            _mediaFiles.Add(new MediaFile(fileName, MediaType.Video));
            SetUIForLatestMediaFile();
            DeleteButton.SetActive(true);
            NewData();
        }
    }

    private void SetUIForLatestMediaFile()
    {
        _currentMediaIndex = _mediaFiles.Count - 1;
        SetMediaFileCounter(_mediaFiles.Count, _mediaFiles.Count);
        if (_currentMediaIndex >= 1)
        {
            PreviousMediaFileButton.gameObject.SetActive(true);
        }
        NextMediaFileButton.gameObject.SetActive(false);
    }

    private void NewData()
    {
        OnNewData?.Invoke();
        //InstructionManager.Instance.Save();
    }

    /// <summary>
    /// Sets the text of the the current selected media file / count of media files
    /// </summary>
    /// <param name="currentMediaNumber">Number of current media file. 1 based counting</param>
    /// <param name="count">the count of media files</param>
    private void SetMediaFileCounter(int currentMediaNumber, int count)
    {
        MediaFileCounterText.text = currentMediaNumber + "/" + count;
    }

    public void OnDeleteCurrentFile()
    {
        _mediaFiles.RemoveAt(_currentMediaIndex);

        if(_currentMediaIndex == 0)
        {
            if(_mediaFiles.Count > 0)
            {

                LoadMediaFile(_mediaFiles[_currentMediaIndex]);

                if (_mediaFiles.Count > 1)
                {
                    NextMediaFileButton.gameObject.SetActive(true);
                }
                else
                {
                    NextMediaFileButton.gameObject.SetActive(false);
                }
                SetMediaFileCounter(_currentMediaIndex+1, _mediaFiles.Count);
            }
            else
            {
                ImageLoader.LoadImageToQuadByFileName();
                PreviousMediaFileButton.gameObject.SetActive(false);
                NextMediaFileButton.gameObject.SetActive(false);
                DeleteButton.SetActive(false);
                SetMediaFileCounter(_currentMediaIndex, _mediaFiles.Count);
            }
        }
        else
        {
            LoadPreivousMediaFile();
        }
        OnNewData?.Invoke();
        //InstructionManager.Instance.Save();

        //if (_currentMediaIndex > 0)
        //{
        //    _currentMediaIndex--;
        //}
    }
}
