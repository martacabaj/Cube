//open cv horseshit and bullshit
#include <imgproc/imgproc.hpp>
#include <core/core.hpp>
#include <highgui/highgui.hpp>

//cpp crap and puke
#include <iostream>
#include <stdio.h>

using namespace std;
using namespace cv;

int thresh = 50, N = 11;
const char* wndname = "Square Detection Demo";
int area;
int globalCounter;
Scalar outputColor;
// helper function:
// finds a cosine of angle between vectors
// from pt0->pt1 and from pt0->pt2
static double angle(Point pt1, Point pt2, Point pt0)
{
    double dx1 = pt1.x - pt0.x;
    double dy1 = pt1.y - pt0.y;
    double dx2 = pt2.x - pt0.x;
    double dy2 = pt2.y - pt0.y;
    return (dx1*dx2 + dy1*dy2) / sqrt((dx1*dx1 + dy1*dy1)*(dx2*dx2 + dy2*dy2) + 1e-10);
}

// returns sequence of squares detected on the image.
// the sequence is stored in the specified memory storage
static void findSquares(const Mat& image, vector<vector<Point> >& squares)
{
    squares.clear();

    Mat pyr, timg, gray0(image.size(), CV_8U), gray;

    // down-scale and upscale the image to filter out the noise
    pyrDown(image, pyr, Size(image.cols / 2, image.rows / 2));
    pyrUp(pyr, timg, image.size());
    vector<vector<Point> > contours;

    // find squares in every color plane of the image
    for (int c = 0; c < 3; c++)
    {
        int ch[] = { c, 0 };
        mixChannels(&timg, 1, &gray0, 1, ch, 1);

        // try several threshold levels
        for (int l = 0; l < N; l++)
        {
            // hack: use Canny instead of zero threshold level.
            // Canny helps to catch squares with gradient shading
            if (l == 0)
            {
                // apply Canny. Take the upper threshold from slider
                // and set the lower to 0 (which forces edges merging)
                Canny(gray0, gray, 0, thresh, 5);
                // dilate canny output to remove potential
                // holes between edge segments
                dilate(gray, gray, Mat(), Point(-1, -1));
            }
            else
            {
                // apply threshold if l!=0:
                //     tgray(x,y) = gray(x,y) < (l+1)*255/N ? 255 : 0
                gray = gray0 >= (l + 1) * 255 / N;
            }

            // find contours and store them all as a list
            findContours(gray, contours, CV_RETR_LIST, CV_CHAIN_APPROX_SIMPLE);

            vector<Point> approx;

            vector<Scalar> mean_colors;
            int counter = 0;

            // test each contour
            for (size_t i = 0; i < contours.size(); i++)
            {
                // approximate contour with accuracy proportional
                // to the contour perimeter
                approxPolyDP(Mat(contours[i]), approx, arcLength(Mat(contours[i]), true)*0.02, true);

                // square contours should have 4 vertices after approximation
                // relatively large area (to filter out noisy contours)
                // and be convex.
                // Note: absolute value of an area is used because
                // area may be positive or negative - in accordance with the
                // contour orientation
                if (approx.size() == 4 &&
                    fabs(contourArea(Mat(approx))) > 1000 && fabs(contourArea(Mat(approx))) <area &&
                    isContourConvex(Mat(approx)))
                {
                    double maxCosine = 0;

                    for (int j = 2; j < 5; j++)
                    {
                        // find the maximum cosine of the angle between joint edges
                        double cosine = fabs(angle(approx[j % 4], approx[j - 2], approx[j - 1]));
                        maxCosine = MAX(maxCosine, cosine);
                    }

                    // if cosines of all angles are small
                    // (all angles are ~90 degree) then write quandrange
                    // vertices to resultant sequence
                    //if it is ok, add mean to mean value and increment counter
                    if (maxCosine < 0.3){
                        squares.push_back(approx);
                        Mat mask_image(image.size(), CV_8U, Scalar(0));
                        drawContours(mask_image, contours, i, Scalar(255), CV_FILLED);
                        mean_colors.push_back(mean(image, mask_image));
                        counter++;
                    }
                }
            }

            Scalar final;
            for (int i = 0; i<mean_colors.size(); i++){
                final[0] += mean_colors.at(i)[0];
                final[1] += mean_colors.at(i)[1];
                final[2] += mean_colors.at(i)[2];
                final[3] += mean_colors.at(i)[3];
            }
            if (counter != 0)
            {
                globalCounter++;
                outputColor[0] += final[0] / counter;
                outputColor[1] += final[1] / counter;
                outputColor[2] += final[2] / counter;
                outputColor[3] += final[3] / counter;

            }
        }
    }
}


// the function draws all the squares in the image
static void drawSquares(Mat& image, const vector<vector<Point> >& squares)
{
    for (size_t i = 0; i < squares.size(); i++)
    {
        const Point* p = &squares[i][0];
        int n = (int)squares[i].size();
        polylines(image, &p, &n, 1, true, Scalar(0, 255, 0), 3, CV_AA);
    }

    imshow(wndname, image);
}

int getColor()
{
    CvCapture* capture = 0;
    capture = cvCaptureFromCAM(0); //0=default, -1=any camera, 1..99=your camera
    if (!capture)
    {
        return -1;
    }

    vector<vector<Point> > squares;

    if (capture)
    {
        for (int i = 0; i<3; i++)
        {
            Mat image = cvQueryFrame(capture);
            area = (image.cols*image.rows) / 2;
            findSquares(image, squares);
            if (waitKey(10) >= 0){
                cvReleaseCapture(&capture);
                cvDestroyWindow(wndname);
            }
        }
        if (globalCounter != 0){
            outputColor[0] = outputColor[0] / globalCounter;
            outputColor[1] = outputColor[1] / globalCounter;
            outputColor[2] = outputColor[2] / globalCounter;
            outputColor[3] = outputColor[3] / globalCounter;
        }
    }
    return 0;
}

//int main( int argc, const char** argv ) //remnant if we will use args at some point
int main()
{
    //outputColor = { 0, 0, 0, 0 };
    //globalCounter = 0;
    //create capture and if capture is not availible print no camera detected
    CvCapture* capture = 0;
    capture = cvCaptureFromCAM(0); //0=default, -1=any camera, 1..99=your camera
    if (!capture) cout << "No camera detected" << endl;

    //created named window that will autosize to size of camera capture

    vector<vector<Point> > squares;

    //start capture in loop if possible
    if (capture)
    {
        //for infinity do
        for (int i = 0; i<3; i++)
        {
            //show captured image from capture(one frame) in result window
            Mat image = cvQueryFrame(capture);
            area = (image.cols*image.rows) / 2;
            findSquares(image, squares);
            drawSquares(image, squares);

            if (waitKey(10) >= 0){
                cvReleaseCapture(&capture);
                cvDestroyWindow(wndname);
            }
        }
        if (globalCounter != 0){
            outputColor[0] = outputColor[0] / globalCounter;
            outputColor[1] = outputColor[1] / globalCounter;
            outputColor[2] = outputColor[2] / globalCounter;
            outputColor[3] = outputColor[3] / globalCounter;
            cout << outputColor << endl;
        }

        return 0;
    }
}
