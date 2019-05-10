using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Controller : MonoBehaviour
{
    const int LEFT = 0;
    const int UP = 1;
    const int DIAG = 2;

    public TMPro.TMP_InputField string1Field;
    public TMPro.TMP_InputField string2Field;
    public Canvas gridCanvas;
    public Canvas controlCanvas;
    public GameObject iconPrefab;
    public GameObject line;

    public Button startButton;
    public GameObject restartButton;
    public GameObject quitButton;
   

    public TMPro.TextMeshProUGUI outputText;
    public TMPro.TextMeshProUGUI autoButtonText;

    public bool simStarted = false;

    public bool automatic = false;
    public float stepTime = .5f;
    private float stepTimeRemaining = .5f;

    public List<List<GameObject>> icons;
    public List<List<int>> backtrace;
    public string result;

    private IEnumerator<string> runResults;
    private Color orange;
    private Color lightGreen;
    private string s1Text;
    private string s2Text;

    // Start is called before the first frame update
    void Start()
    {
        controlCanvas.gameObject.SetActive(false);
        gridCanvas.gameObject.SetActive(false);
        icons = new List<List<GameObject>>();
        orange = new Color(1f, 204f / 255, 153f / 255);
        lightGreen = new Color(.66f, .94f, .82f);
        result = "";
        automatic = false;
        restartButton.SetActive(false);
        quitButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(automatic)
        {
            stepTimeRemaining -= Time.deltaTime;
            if(stepTimeRemaining < 0.0f)
            {
                RunStep();
                stepTimeRemaining = stepTime;
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleAutomatic()
    {
        Debug.Log("Switching automatic modes");
        automatic = !automatic;
        if(automatic)
        {
            autoButtonText.text = "Stop automatic run";
        } else
        {
            autoButtonText.text = "Run automatically";
        }
    }

    public void RunStep()
    {
        string output = "";
        if(runResults.MoveNext())
        {
            output = runResults.Current;
        } else
        {
            output = result;
        }
        outputText.text = output;
    }

    public IEnumerator<string> EditDistance()
    {
        Vector3 oneZ = new Vector3(0, 0, -.01f);
        LineRenderer lr1 = Instantiate(line, gridCanvas.transform).GetComponent<LineRenderer>();
        lr1.SetPosition(0, icons[1][1].transform.position + oneZ);
        lr1.sortingOrder = 4; lr1.sortingLayerName = "UI";
        for (int j = 1; j < icons[1].Count; j++)
        {
            
            TMPro.TextMeshProUGUI t = icons[1][j].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            t.text = (j - 1).ToString();
            icons[1][j].GetComponent<Image>().color = Color.green;
        }
        lr1.SetPosition(1, icons[1][icons[1].Count - 1].transform.position + oneZ);

        yield return "Setting intial values of first row.";
        for (int j = 1; j < icons[1].Count; j++)
        {
            TMPro.TextMeshProUGUI t = icons[1][j].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            icons[1][j].GetComponent<Image>().color = Color.white;
        }


        LineRenderer lr2 = Instantiate(line, gridCanvas.transform).GetComponent<LineRenderer>();
        lr2.SetPosition(0, icons[1][1].transform.position + oneZ);
        lr2.sortingOrder = 4; lr2.sortingLayerName = "UI";
        for (int i = 1; i < icons.Count; i++)
        {
            var t = icons[i][1].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            t.text = (i - 1).ToString();
            icons[i][1].GetComponent<Image>().color = Color.green;
            backtrace[i - 1][0] = UP;
        }
        lr2.SetPosition(1, icons[icons.Count - 1][1].transform.position + oneZ);

        yield return "Setting intial values of first column.";
        for (int i = 1; i < icons.Count; i++)
        {
            icons[i][1].GetComponent<Image>().color = Color.white;
        }
        Image upPrev = null;
        Image leftPrev = null;
        Image diagPrev = null;
        Image currPrev = null;

        for (int i = 2; i < icons.Count; i++)
        {
            for(int j = 2; j < icons[i].Count; j++)
            {
                if(upPrev != null)
                {
                    upPrev.color = Color.white;
                }
                if(leftPrev != null)
                {
                    leftPrev.color = Color.white;
                }
                if(diagPrev != null)
                {
                    diagPrev.color = Color.white;
                }
                if(currPrev != null)
                {
                    currPrev.color = Color.white;
                }

                string retString;
                char c1 = s1Text[j - 2];
                char c2 = s2Text[i - 2];
                int cost;
                if (s1Text[j - 2] == s2Text[i - 2])
                {
                    retString = "Characters '" + c1 + "' and '" + c2 + "' are the same, so there will be no replacement cost for the diagonal consideration.";
                    cost = 0;
                } else
                {
                    retString = "Characters '" + c1 + "' and '" + c2 + "' are the different.";
                    cost = 1;
                }

                retString += "\n";

                var up = icons[i-1][j].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                var upc = icons[i - 1][j].GetComponent<Image>();
                int upv = System.Int32.Parse(up.text) + 1;

                var left = icons[i][j-1].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                var leftc = icons[i][j - 1].GetComponent<Image>();
                int leftv = System.Int32.Parse(left.text) + 1;

                var diag = icons[i-1][j - 1].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                var diagc = icons[i-1][j - 1].GetComponent<Image>();
                int diagv = System.Int32.Parse(diag.text) + cost;

                upc.color = orange;
                leftc.color = orange;
                diagc.color = orange;

                var curr = icons[i][j].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                var currc = icons[i][j].GetComponent<Image>();
                currc.color = Color.green;

                upPrev = upc;
                leftPrev = leftc;
                diagPrev = diagc;
                currPrev = currc;

                

                LineRenderer r = Instantiate(line, gridCanvas.transform).GetComponent<LineRenderer>();
                r.SetPosition(0, curr.transform.position + oneZ);
                r.sortingOrder = 4; r.sortingLayerName = "UI";

                if (upv < leftv && upv < diagv)
                {
                    retString += "Adding a gap character to the first string is the best option.";
                    curr.text = upv.ToString();
                    r.SetPosition(1, up.transform.position + oneZ);
                    backtrace[i - 1][j - 1] = UP;
                    

                } else if(leftv < upv && leftv < diagv)
                {
                    retString += "Adding a gap character to the second string is the best option.";
                    curr.text = leftv.ToString();
                    r.SetPosition(1, left.transform.position + oneZ);
                    backtrace[i - 1][j - 1] = LEFT;

                } else
                {
                    retString += "Replacing/matching the character is the best option.";
                    curr.text = diagv.ToString();
                    r.SetPosition(1, diag.transform.position + oneZ);
                    backtrace[i - 1][j - 1] = DIAG;
                }
                yield return retString;

            }
            if (upPrev != null)
            {
                upPrev.color = Color.white;
            }
            if (leftPrev != null)
            {
                leftPrev.color = Color.white;
            }
            if (diagPrev != null)
            {
                diagPrev.color = Color.white;
            }
            if (currPrev != null)
            {
                currPrev.color = Color.white;
            }
        }

        int ii = backtrace.Count - 1;
        int jj = backtrace[0].Count - 1;
        string s1New = "";
        string s2New = "";
        while (ii != 0 || jj != 0)
        {
            icons[ii + 1][jj + 1].GetComponent<Image>().color = lightGreen;
            int direction = backtrace[ii][jj];
            if (direction == UP)
            {
                s1New = "-" + s1New;
                s2New = s2Text[ii - 1] + s2New;
                ii--;
            }
            if (direction == LEFT)
            {
                s1New = s1Text[jj - 1] + s1New;
                s2New = "-" + s2New;
                jj--;
            }
            if(direction == DIAG) {
                s1New = s1Text[jj - 1] + s1New;
                s2New = s2Text[ii - 1] + s2New;
                ii--;
                jj--;
            }
            if (ii == 0 && jj == 0)
            {
                icons[ii + 1][jj + 1].GetComponent<Image>().color = lightGreen;
                restartButton.SetActive(true);
                quitButton.SetActive(true);
            }
            yield return s1New + "\n" + s2New;

        }
        result = s1New + "\n" + s2New;
        
        yield return result;
    }

    public void SimStart()
    {
        if (simStarted)
        {
            Debug.Log("Sim already started");
            return;
        }
        if (string1Field.text.Length < 1 || string2Field.text.Length < 1)
        {
            Debug.Log("Please enter strings to be matched");
            return;
        }
        startButton.interactable = false;
        s1Text = string1Field.text;
        s2Text = string2Field.text;
        simStarted = true;

        controlCanvas.gameObject.SetActive(true);
        gridCanvas.gameObject.SetActive(true);

        RectTransform gridCanvasRectTransform = gridCanvas.GetComponent<RectTransform>();
        gridCanvasRectTransform.sizeDelta = new Vector2(string1Field.text.Length * 30 + 60, string2Field.text.Length * 30 + 60);

        backtrace = new List<List<int>>();  
        for(int i = 0; i < s2Text.Length + 1; i++)
        {
            backtrace.Add(new List<int>());
            for(int j = 0; j < s1Text.Length + 1; j++)
            {
                backtrace[i].Add(LEFT);
            }
        }

        for(int i = 0; i < s2Text.Length + 2; i++)
        {
            icons.Add(new List<GameObject>());
            for(int j = 0; j < s1Text.Length + 2; j++)
            {
             
                icons[i].Add(Instantiate(iconPrefab, gridCanvas.transform));
                if(i == 0 && j > 1)
                {
                    var txt = icons[i][j].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    txt.text = s1Text[j - 2].ToString();
                    txt.color = Color.white;
                    txt.fontSize = 20;

                    icons[i][j].GetComponent<Image>().color = Color.blue;
                }

                else if (i > 1 && j == 0)
                {
                    var txt = icons[i][j].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    txt.text = s2Text[i - 2].ToString();
                    txt.color = Color.white;
                    txt.fontSize = 20;

                    icons[i][j].GetComponent<Image>().color = Color.blue;
                }

                else if(i == 0 || j == 0)
                {
                    var txt = icons[i][j].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    txt.text = "";
                    txt.color = Color.white;
                    txt.fontSize = 20;

                    icons[i][j].GetComponent<Image>().color = Color.blue;
                }
            }
        }

        runResults = EditDistance();
    }

    public void Quit()
    {
        Application.Quit();
    }

}
