using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common.Formation;

public class TacticCardController : MonoBehaviour
{
    public int unitCount = 8;
    public float slotSize = 1f;
    private float? slotSizeOld = null;
    public float slotMargin = 0.1f;
    private float? slotMarginOld = null;
    public Transform slotPrefab;
    public enum SlotType
    {
        Rectangle,
        Square,
        Hexagon,
        Rhombus,
        TriangleDown,
        TriangleUp
    }
    public SlotType slotType = SlotType.Square;
    private SlotType? slotTypeOld = null;

    private Vector2[] slotPositions;
    private Dictionary<SlotType, TacticCardSlotController[]> slotsDictionary = null;
    private Vector2 centerOfMass;
    private bool onDrag = false;
    private DragActions dragAction = DragActions.Select;
    public enum DragActions
    {
        Select,
        Unselect
    }
    private List<TacticCardSlotController> selectedSlots = new List<TacticCardSlotController>();
    private string cardName;
    private Vector2 totalPos = Vector2.zero;
    private int totalSlotCount = 0;

    [Range(0, 360)]
    public int cardAngle = 0;
    private int? cardAngleOld = null;

    public enum GridDirections
    {
        FlatTop,
        PointyTop
    }
    public GridDirections gridDirection = GridDirections.FlatTop;
    private GridDirections? gridDirectionOld = null;

    public void addSelectedSlot(TacticCardSlotController selectedSlot)
    {
        int selectedIndex = selectedSlots.IndexOf(selectedSlot);
        if (selectedIndex > -1)
        {
            return;
        }

        selectedSlots.Add(selectedSlot);

        selectedSlot.setSlotPositionString(selectedSlots.Count);
    }

    public void removeSelectedSlot(TacticCardSlotController selectedSlot)
    {
        int selectedIndex = selectedSlots.IndexOf(selectedSlot);
        if (selectedIndex == -1)
        {
            return;
        }

        selectedSlots.RemoveAt(selectedIndex);

        selectedSlot.deactiveSlotPositionString();

        int idx = 1;
        foreach (TacticCardSlotController slot in selectedSlots)
        {
            slot.setSlotPositionString(idx);
            idx++;
        }
    }

    public bool unitMaxCountReached()
    {
        return selectedSlots.Count == unitCount;
    }

    public void startPointerDrag(DragActions dragAction)
    {
        onDrag = true;
        this.dragAction = dragAction;
    }

    public void endPointerDrag()
    {
        onDrag = false;
    }

    public bool pointerOnDrag()
    {
        return onDrag;
    }

    public DragActions getDragAction()
    {
        return dragAction;
    }

    private void makeGrid()
    {
        slotTypeOld = slotType;
        cardAngleOld = cardAngle;
        slotSizeOld = slotSize;
        slotMarginOld = slotMargin;
        gridDirectionOld = gridDirection;

        if (slotType == SlotType.Rectangle)
        {
            int yAxisUnitCount = (int)(Mathf.Round((float)unitCount / 1.618f));
            makeRectangularShape(0, 6, 0, 4);
        }
        else if (slotType == SlotType.Square)
        {
            makeRectangularShape(0, 5, 0, 5);
        }
        else if (slotType == SlotType.Hexagon)
        {
            makeHexagonalShape(3);
        }
        else if (slotType == SlotType.Rhombus)
        {
            makeRhombusShape(6);
        }
        else if (slotType == SlotType.TriangleDown)
        {
            makeDownTriangularShape(6);
        }
        else if (slotType == SlotType.TriangleUp)
        {
            makeUpTriangularShape(6);
        }
    }

    public void Start()
    {
        makeGrid();
        renderGrid();
    }

    public void Update()
    {
        Vector3 euler = transform.eulerAngles;
        euler.z = cardAngle;
        transform.eulerAngles = euler;

        if (tacticCardSettingsUpdated())
        {
            makeGrid();
            renderGrid();
        }
    }

    private bool tacticCardSettingsUpdated()
    {
        bool slotTypeChanged = slotTypeOld != null && slotTypeOld != slotType;
        bool cardAngleChanged = cardAngleOld != null && cardAngleOld != cardAngle;
        bool slotSizeChanged = slotSizeOld != null && slotSizeOld != slotSize;
        bool slotMarginChanged = slotMarginOld != null && slotMarginOld != slotMargin;
        bool gridDirectionChanged = gridDirectionOld != null && gridDirectionOld != gridDirection;
        return slotTypeChanged || cardAngleChanged || slotSizeChanged || slotMarginChanged || gridDirectionChanged;
    }

    private TacticCardSlotController[] makeSlots()
    {
        TacticCardSlotController[] slots = new TacticCardSlotController[totalSlotCount];
        centerOfMass = totalPos / slotPositions.Length;
        int index = 0;
        foreach (Vector2 slotPosition in slotPositions)
        {
            Transform slot = Instantiate(slotPrefab, slotPosition - centerOfMass, Quaternion.identity);
            slot.gameObject.SetActive(false);
            slot.localScale = Vector3.one * slotSize;
            slot.parent = transform;
            TacticCardSlotController slotController = slot.GetComponent<TacticCardSlotController>();
            slots[index] = slotController;
            index++;
        }

        return slots;
    }

    private void renderGrid()
    {
        centerOfMass = totalPos / slotPositions.Length;

        if (slotsDictionary == null)
        {
            slotsDictionary = new Dictionary<SlotType, TacticCardSlotController[]>();
        }

        if (!slotsDictionary.ContainsKey(slotType))
        {
            TacticCardSlotController[] slots = makeSlots();
            slotsDictionary.Add(slotType, slots);
        }

        foreach (KeyValuePair<SlotType, TacticCardSlotController[]> entry in slotsDictionary)
        {
            if (entry.Key == slotType)
            {
                int i = 0;
                foreach (TacticCardSlotController slotController in entry.Value)
                {
                    slotController.gameObject.SetActive(true);
                    slotController.transform.position = slotPositions[i] - centerOfMass;
                    slotController.transform.localScale = Vector3.one * slotSize;
                    i++;
                }
            }
            else
            {
                foreach (TacticCardSlotController slotController in entry.Value)
                {
                    slotController.gameObject.SetActive(false);
                }
            }
        }
    }
    // function makeRectangularShape(minCol, maxCol, minRow, maxRow, convert) {
    //     let results = [];
    //     for (let col = minCol; col <= maxCol; col++) {
    //         for (let row = minRow; row <= maxRow; row++) {
    //             let hex = convert(new OffsetCoord(col, row));
    //             hex.col = col;
    //             hex.row = row;
    //             results.push(hex);
    //         }
    //     }
    //     return results;
    // }
    private void makeRectangularShape(int minCol, int maxCol, int minRow, int maxRow)
    {
        HexagonalGridData hexagonalGridData = new HexagonalGridData();
        hexagonalGridData.points = new List<HexagonalPoint>();
        for (int col = minCol; col <= maxCol; col++)
        {
            for (int row = minRow; row <= maxRow; row++)
            {
                float q = col - (row - (row & 1)) / 2;
                float r = row;

                HexagonalPoint hex = new HexagonalPoint();
                hex.q = q;
                hex.r = r;

                hexagonalGridData.points.Add(hex);
            }
        }

        makeHexagonGrid(hexagonalGridData);
    }

    private void makeHexagonalShape(int N)
    {
        HexagonalGridData hexagonalGridData = new HexagonalGridData();
        hexagonalGridData.points = new List<HexagonalPoint>();
        for (int q = -N; q <= N; q++)
        {
            for (int r = -N; r <= N; r++)
            {
                HexagonalPoint hex = new HexagonalPoint();
                hex.q = q;
                hex.r = r;

                if (hex.len() <= N)
                {
                    hexagonalGridData.points.Add(hex);
                }
            }
        }

        makeHexagonGrid(hexagonalGridData);
    }

    private void makeRhombusShape(int w, int? h = null)
    {
        if (h == null)
        {
            h = w;
        }
        HexagonalGridData hexagonalGridData = new HexagonalGridData();
        hexagonalGridData.points = new List<HexagonalPoint>();
        for (int r = 0; r < h; r++)
        {
            for (int q = 0; q < w; q++)
            {
                HexagonalPoint hex = new HexagonalPoint();
                hex.q = q;
                hex.r = r;

                hexagonalGridData.points.Add(hex);
            }
        }

        makeHexagonGrid(hexagonalGridData);
    }

    private void makeDownTriangularShape(int N)
    {
        HexagonalGridData hexagonalGridData = new HexagonalGridData();
        hexagonalGridData.points = new List<HexagonalPoint>();
        for (int r = 0; r < N; r++)
        {
            for (int q = 0; q < N - r; q++)
            {
                HexagonalPoint hex = new HexagonalPoint();
                hex.q = q;
                hex.r = r;

                hexagonalGridData.points.Add(hex);
            }
        }

        makeHexagonGrid(hexagonalGridData);
    }

    private void makeUpTriangularShape(int N)
    {
        HexagonalGridData hexagonalGridData = new HexagonalGridData();
        hexagonalGridData.points = new List<HexagonalPoint>();
        for (int r = 0; r < N; r++)
        {
            for (int q = N - r - 1; q < N; q++)
            {
                HexagonalPoint hex = new HexagonalPoint();
                hex.q = q;
                hex.r = r;

                hexagonalGridData.points.Add(hex);
            }
        }

        makeHexagonGrid(hexagonalGridData);
    }

    private void makeHexagonGrid(HexagonalGridData hexagonalGridData)
    {
        // HexagonalGridData hexagonalGridData = JsonUtility.FromJson<HexagonalGridData>(Common.DataService.LoadDefaultGameData("hexagonalGrids.json"));
        totalSlotCount = hexagonalGridData.points.Count;
        slotPositions = new Vector2[totalSlotCount];
        totalPos = Vector2.zero;

        float slotMarginOfCenter = slotSize + slotMargin;
        int index = 0;
        foreach (HexagonalPoint hex in hexagonalGridData.points)
        {
            float x = 0f, y = 0f;
            if (gridDirection == GridDirections.FlatTop)
            {
                x = slotMarginOfCenter * (Mathf.Sqrt(3) * hex.q + Mathf.Sqrt(3) / 2 * hex.r);
                y = slotMarginOfCenter * (3.0f / 2 * hex.r);
            }
            else if (gridDirection == GridDirections.PointyTop)
            {
                x = slotMarginOfCenter * (3.0f / 2 * hex.q);
                y = slotMarginOfCenter * (Mathf.Sqrt(3) * hex.r + Mathf.Sqrt(3) / 2 * hex.q);
            }

            Vector2 slotPosition = Quaternion.Euler(0, 0, cardAngle) * (new Vector2(x, y));
            slotPositions[index] = slotPosition;
            index++;
            totalPos += slotPosition;
        }
    }

    public void setCardName(string cardName)
    {
        this.cardName = cardName;
    }

    public void saveFormationData()
    {
        FormationData formationData = JsonUtility.FromJson<FormationData>(Common.DataService.LoadGameData("formations.json"));

        if (formationData == null)
        {
            formationData = new FormationData();
        }

        Formation formation = new Formation(formationData.formations.Count, cardName);

        foreach (TacticCardSlotController slot in selectedSlots)
        {
            Vector3 pos = new Vector3(slot.transform.position.x, slot.transform.position.z, slot.transform.position.y);
            formation.addPoint(pos);
        }

        formationData.formations.Add(formation);

        Common.DataService.SaveGameData("formations.json", JsonUtility.ToJson(formationData));

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
