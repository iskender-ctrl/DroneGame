using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Formation;
using UnityEngine.SceneManagement;
using System.Linq;

public class FormationToggle
{
    public Toggle toggle;
    public Formation formation;
}

public class TacticCardMenuController : MonoBehaviour
{
    public RectTransform prefab;
    public ScrollRect scrollView;
    public RectTransform content;
    public int defaultAutoToggleCount = 3;
    public bool showJustSelected = false;
    public PlatoonController platoonController;
    private List<FormationToggle> formationToggles = new List<FormationToggle>();

    public void Start()
    {
        StartCoroutine(FetchItemModelIsFromServer(results => OnReceivedNewFormations(results)));
    }

    private void OnReceivedNewFormations(List<Formation> formations)
    {
        int idx = 0;
        foreach (Formation formation in formations)
        {
            GameObject listItem = Instantiate(prefab.gameObject) as GameObject;
            listItem.transform.SetParent(content, false);

            Toggle toggle = listItem.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = idx < defaultAutoToggleCount;

                FormationToggle ft = new FormationToggle();
                ft.formation = formation;
                ft.toggle = toggle;

                formationToggles.Add(ft);
            }

            Button button = listItem.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(delegate
                {
                    platoonController.setFormation(formation);
                });
            }

            Text text = listItem.GetComponentInChildren<Text>();
            text.text = formation.getName();

            listItem.SetActive(true);

            idx++;
        }

        Formation defaultFormationItem = formations.LastOrDefault();
        if(defaultFormationItem != null && platoonController != null) {
            platoonController.Start();
            platoonController.setFormation(defaultFormationItem);
            platoonController.makePlatoon();
        }
    }

    public IEnumerator<List<Formation>> FetchItemModelIsFromServer(Action<List<Formation>> onDone)
    {
        FormationData formationData = JsonUtility.FromJson<FormationData>(Common.DataService.LoadGameData("formations.json"));

        List<Formation> filteredFormations = new List<Formation>();
        if (showJustSelected)
        {
            SelectedTacticCards selectedTacticCards = JsonUtility.FromJson<SelectedTacticCards>(Common.DataService.LoadGameData("selectedFormations.json"));
            foreach (int cardIdx in selectedTacticCards.selectedTacticCards)
            {
                filteredFormations.Add(formationData.formations[cardIdx]);
            }
        }
        else
        {
            filteredFormations = formationData.formations;
        }

        onDone(filteredFormations);

        yield return filteredFormations;
    }

    public void saveSelectedCards()
    {
        int idx = 0;
        SelectedTacticCards selectedTacticCards = new SelectedTacticCards();
        foreach (FormationToggle formationToggle in formationToggles)
        {
            Debug.Log(formationToggle.formation.getName() + ": " + formationToggle.toggle.isOn.ToString());
            if (formationToggle.toggle.isOn)
            {
                selectedTacticCards.addTacticCard(idx);
            }

            idx++;
        }

        Common.DataService.SaveGameData("selectedFormations.json", JsonUtility.ToJson(selectedTacticCards));
    }

    public void PlayGame()
    {
        // Save selected tactic cards beforea load war scene
        saveSelectedCards();

        // Go to war scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
}
