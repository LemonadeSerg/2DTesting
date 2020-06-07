using UnityEngine;

public class StepProjection : MonoBehaviour
{
    public GameObject[] curveG;
    public Vector2[] curveV;

    public GameObject leftFoot, rightFoot;

    public float projectionResoultion = 10f;
    public float stepPeak = 1f;
    public float stepDistance = 1f;
    public float footDown = 0.5f;
    public float gradient;

    public Vector2 plantedLeftVector, plantedRightVector;
    private RaycastHit2D Projection = new RaycastHit2D();

    private float xAxis;
    public bool moving;
    public float moveSpeed;
    public float movingAmount;

    public GameObject MovingFoot;
    public bool footInAir;
    public int lastDir;

    // Start is called before the first frame update
    private void Start()
    {
        PlantFeet();
    }

    private void PlantFeet()
    {
        if (leftFoot.transform.position.x < rightFoot.transform.position.x)
        {
            plantedLeftVector = leftFoot.transform.position;
            plantedRightVector = rightFoot.transform.position;
        }
        else
        {
            plantedRightVector = leftFoot.transform.position;
            plantedLeftVector = rightFoot.transform.position;
        }
    }

    private float GetValidStepDistance()
    {
        curveV = new Vector2[4];
        float validCurveDistanceMulti = 0;
        for (float i = 0; i < 1; i += 1 / projectionResoultion)
        {
            if (xAxis > 0 && lastDir >= 0)
            {
                curveV[0] = plantedLeftVector;
                curveV[1] = plantedRightVector + Vector2.up * stepPeak;
                curveV[2] = plantedRightVector + Vector2.right * stepDistance * (1 - i);
                curveV[3] = plantedRightVector + Vector2.down * footDown;
                Projection = MultiCurveCast(curveV, true);
            }
            if (xAxis < 0 && lastDir <= 0)
            {
                curveV[0] = plantedRightVector;
                curveV[1] = plantedLeftVector + Vector2.up * stepPeak;
                curveV[2] = plantedLeftVector + Vector2.left * stepDistance * (1 - i);
                curveV[3] = plantedLeftVector + Vector2.down * footDown;
                Projection = MultiCurveCast(curveV, false);
            }

            if (Projection.collider != null)
                if (IsGradValid(getGradientFromVector(Projection.point)))
                {
                    Debug.DrawRay(Projection.point, Vector2.up, Color.green);
                    validCurveDistanceMulti = i;
                    break;
                }
        }

        return validCurveDistanceMulti;
    }

    private GameObject GetMovingFoot(Vector2 dir)
    {
        if (leftFoot.transform.position.x < rightFoot.transform.position.x && dir == Vector2.right)
            return leftFoot;
        else
        if (leftFoot.transform.position.x > rightFoot.transform.position.x && dir == Vector2.right)
            return rightFoot;
        else
        if (leftFoot.transform.position.x < rightFoot.transform.position.x && dir == Vector2.left)
            return rightFoot;
        else
        if (leftFoot.transform.position.x > rightFoot.transform.position.x && dir == Vector2.left)
            return leftFoot;
        else return null;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!moving && (movingAmount <= 0 || movingAmount >= 1))
            PlantFeet();

        xAxis = Input.GetAxis("Horizontal");

        if (GetValidStepDistance() < 1)
        {
            if (xAxis > 0 && lastDir >= 0)
            {
                if (moving == false)
                    MovingFoot = GetMovingFoot(Vector2.right);
                moving = true;
                movingAmount += moveSpeed * Time.deltaTime;
                lastDir = 1;
            }
            else
            if (xAxis < 0 && lastDir <= 0)
            {
                if (moving == false)
                    MovingFoot = GetMovingFoot(Vector2.left);
                moving = true;
                movingAmount += moveSpeed * Time.deltaTime;
                lastDir = -1;
            }
            else
            {
                if (movingAmount < 0.5f)
                    movingAmount -= moveSpeed * 2 * Time.deltaTime;
                else
                    movingAmount += moveSpeed * 2 * Time.deltaTime;
                movingAmount = Mathf.Clamp(movingAmount, 0, 1);
            }
        }

        if (movingAmount > 0 && Vector2.Distance(MovingFoot.transform.position, Projection.point) > 0.1f)
        {
            if (lastDir > 0)
            {
                if (movingAmount < 0.3333f)
                {
                    Debug.Log("Moving 1/3 Path");
                    MovingFoot.transform.position = plantedLeftVector + GetVectorOfPath(movingAmount * 3, plantedLeftVector, plantedRightVector + Vector2.up * stepPeak, true);
                }
                else if (movingAmount < 0.6666f)
                {
                    Debug.Log("Moving 2/3 Path");
                    MovingFoot.transform.position = plantedRightVector + Vector2.up * stepPeak + GetVectorOfPath((movingAmount - 0.3333f) * 3, plantedRightVector + Vector2.up * stepPeak, plantedRightVector + Vector2.right * (stepDistance * (1 - GetValidStepDistance())), true);
                }
                else if (movingAmount < 1)
                {
                    Debug.Log("Moving 3/3 Path");
                    MovingFoot.transform.position = plantedRightVector + Vector2.right * (stepDistance * (1 - GetValidStepDistance())) + GetVectorOfPath((movingAmount - 0.6666f) * 3, plantedRightVector + Vector2.right * (stepDistance * (1 - GetValidStepDistance())), plantedRightVector + Vector2.down * footDown, true);
                }
            }
            else if (lastDir < 0)
            {
                if (movingAmount < 0.3333f)
                {
                    Debug.Log("Moving 1/3 Path");
                    MovingFoot.transform.position = plantedRightVector + GetVectorOfPath(movingAmount * 3, plantedRightVector, plantedLeftVector + Vector2.up * stepPeak, false);
                }
                else if (movingAmount < 0.6666f)
                {
                    Debug.Log("Moving 2/3 Path");
                    MovingFoot.transform.position = plantedLeftVector + Vector2.up * stepPeak + GetVectorOfPath((movingAmount - 0.3333f) * 3, plantedLeftVector + Vector2.up * stepPeak, plantedLeftVector + Vector2.left * (stepDistance * (1 - GetValidStepDistance())), false);
                }
                else if (movingAmount < 1)
                {
                    Debug.Log("Moving 3/3 Path");
                    MovingFoot.transform.position = plantedLeftVector + Vector2.left * (stepDistance * (1 - GetValidStepDistance())) + GetVectorOfPath((movingAmount - 0.6666f) * 3, plantedLeftVector + Vector2.left * (stepDistance * (1 - GetValidStepDistance())), plantedLeftVector + Vector2.down * footDown, false);
                }
            }
        }
        else if (Vector2.Distance(MovingFoot.transform.position, Projection.point) < 0.1f)
        {
            MovingFoot.transform.position = Projection.point + Vector2.up * 0.1f;
            moving = false;
            movingAmount = 0;
            PlantFeet();
            lastDir = 0;
        }

        if (movingAmount <= 0)
        {
            if (lastDir > 0)
                MovingFoot.transform.position = plantedLeftVector;
            else if (lastDir < 0)
                MovingFoot.transform.position = plantedRightVector;
            movingAmount = 0;
            moving = false;
            lastDir = 0;
        }
    }

    //Gets Vector Change
    //From origin to peak by travelx amount
    private Vector2 GetVectorOfPath(float travelx, Vector2 origin, Vector2 peak, bool positive)
    {
        //Calculates the vector change
        Vector2 Difference = peak - origin;

        //Stores stores the value of where along curve is read
        //0 = first half of the curve
        //0.5 = secondHalf
        float startXPer;

        //Depending on the vector change the curve will need to be either the steepRise or smooth Rise
        if (Difference.x > 0 && Difference.y < 0 || Difference.x < 0 && Difference.y > 0)
            if (positive)
                startXPer = 0.5f;
            else
                startXPer = 0f;
        else
            if (positive)
            startXPer = 0f;
        else
            startXPer = 0.5f;

        //travel x is how far along the curve to readfrom as with percentage input
        //E.G. total distance is 3ft want to read y value of half way
        //testX = 0.5(50%) * 3 = 1.5ft
        float testX = travelx * Difference.x;

        //Basic Math Curve
        float y = -(((testX + 2 * startXPer * Difference.x) - Difference.x) / (Difference.x * Difference.x)) * (((testX + 2 * startXPer * Difference.x) - Difference.x) * Difference.y) + Difference.y;

        //if startXPer == 0.5 yAxis needs flipping
        if (startXPer == 0.5)
            return new Vector2(testX, Difference.y - y);
        else
            return new Vector2(testX, y);
    }

    //Take sample cast from either side of point
    //(y-y)/(x-x) = gradient
    //if either collider fails or it's not withing distance
    //returns 999 used as invalid
    private float getGradientFromVector(Vector2 point)
    {
        float seperation = 0.05f;
        float lift = 0.1f;
        float down = 0.1f;

        RaycastHit2D rh = Linecast(point + Vector2.right * seperation + Vector2.up * lift, point + Vector2.right * seperation + Vector2.down * down, Color.black);
        RaycastHit2D lh = Linecast(point - Vector2.right * seperation + Vector2.up * lift, point - Vector2.right * seperation + Vector2.down * down, Color.black);

        if (rh.collider == null || lh.collider == null)
            return 999;
        else
        if (rh.collider.gameObject != lh.collider.gameObject)
            return 999;
        else
            return (rh.point.y - lh.point.y) / (rh.point.x - lh.point.x);
    }

    //Checks if gradient is within a limit
    private bool IsGradValid(float grad)
    {
        float limit = 1.3f; //1f = 45Degres
        gradient = grad;
        if (grad > -limit && grad < limit)
            return true;
        else
            return false;
    }

    //Projects curveCast across 2+ points using Vector2 Points
    //Offset allows for Shrinking Rays Distance from origin
    private RaycastHit2D MultiCurveCast(Vector2[] points, bool positive)
    {
        RaycastHit2D rh = new RaycastHit2D();
        if (points.Length > 1)
            rh = CurveCast(points[0], points[0 + 1], positive);
        if (rh.collider != null)
            return rh;
        for (int i = 1; i < points.Length - 1; i++)
        {
            rh = CurveCast(points[i], points[i + 1], positive);
            if (rh.collider != null)
                return rh;
        }
        return rh;
    }

    //Projects curveCast across 2+ points using gameObjects to detarmin points
    private RaycastHit2D MultiCurveCast(GameObject[] points, bool positive)
    {
        Vector2[] Vpoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Vpoints[i] = points[i].transform.position;
        }
        return MultiCurveCast(Vpoints, positive);
    }

    //Projects Linecast across a curve through two points
    //postive true will push curve away from local theoretical center
    //postive false will pull crurve towards local theoretical center
    private RaycastHit2D CurveCast(Vector2 point1, Vector2 point2, bool positive)
    {
        RaycastHit2D rh = new RaycastHit2D();
        for (float x = 0f; x < 1; x += (1f / projectionResoultion))
        {
            rh = Linecast(
                point1 + GetVectorOfPath(x,
                point1,
                point2, positive),
                point1 + GetVectorOfPath(x + (1f / projectionResoultion),
                point1,
                point2, positive), Color.red);
            if (rh.collider != null)
                return rh;
        }
        return rh;
    }

    //Simply draws a debug line when using linecast
    private RaycastHit2D Linecast(Vector2 start, Vector2 end)
    {
        Debug.DrawLine(start, end);
        return Physics2D.Linecast(start, end);
    }

    private RaycastHit2D Linecast(Vector2 start, Vector2 end, Color color)
    {
        Debug.DrawLine(start, end, color);
        return Physics2D.Linecast(start, end);
    }
}