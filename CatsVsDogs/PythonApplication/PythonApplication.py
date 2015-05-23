import cv2
import numpy as np
import os
import time
def main2(filename):
    img = cv2.imread(filename)
    gray = cv2.imread(filename,0)

    ret,thresh = cv2.threshold(gray,127,255,1)
               
    contours,h = cv2.findContours(thresh, cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)#cv2.findContours(thresh,1,2)
               
    for cnt in contours.iteritems():
        approx = cv2.approxPolyDP(cnt,0.01 * cv2.arcLength(cnt,True),True)
        print len(approx)
        if len(approx) == 5:
            print "pentagon"
            cv2.drawContours(img,[cnt],0,255,-1)
        elif len(approx) == 3:
            print "triangle"
            cv2.drawContours(img,[cnt],0,(0,255,0),-1)
        elif len(approx) == 4:
            print "square"
            cv2.drawContours(img,[cnt],0,(0,0,255),-1)
        elif len(approx) == 9:
            print "half-circle"
            cv2.drawContours(img,[cnt],0,(255,255,0),-1)
        elif len(approx) > 15:
            print "circle"
            cv2.drawContours(img,[cnt],0,(0,255,255),-1)

    cv2.imshow('img',img)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

def main():
    directory = r'E:/Uczelnia/sem6/BIAI/train/dogs'
    lastfile = r'cat.2.jpg'
    iteration = 0
    amountOfDetected = 0
    for file in os.listdir(directory):       
        print iteration
        iteration+=1
        amountOfDetected += prepare_single_file(directory, file, True)
    print amountOfDetected

minR = 10
maxR = 20
def prepare_single_file(directory,filename, show):
    img = cv2.imread(os.path.normcase(directory + "//" + filename), 0)
    img = cv2.medianBlur(img,5)
    tmpimg = cv2.imread(os.path.normcase(directory + "//" + filename))

    cimg = cv2.cvtColor(img,cv2.COLOR_GRAY2BGR)
    circles = cv2.HoughCircles(img,cv2.HOUGH_GRADIENT,1,10,param1=50,param2=30,minRadius=minR,maxRadius=maxR)
    if circles != None:
        if show == True:
            circles = np.uint16(np.around(circles))
            for i in circles[0,:]:
                # draw the outer circle
                cv2.circle(tmpimg,(i[0],i[1]),i[2],(0,255,0),2)
                # draw the center of the circle
                cv2.circle(tmpimg,(i[0],i[1]),2,(0,0,255),3)

            cv2.imshow(filename,tmpimg)
            cv2.waitKey(0)
            cv2.destroyAllWindows()
        return 1
    return 0

start_time = time.time()
directory = r'E:/Uczelnia/sem6/BIAI/train/dogs/cat.2.jpg'
lastfile = r'cat.2.jpg'
main()

print("--- %s seconds ---" % (time.time() - start_time))
#img = cv2.imread('opencv_logo2.jpg',0)
#img = cv2.imread(os.path.normcase(directory + "//" + lastfile), 0)
#img = cv2.medianBlur(img,5)
#cimg = cv2.cvtColor(img,cv2.COLOR_GRAY2BGR)

#circles = cv2.HoughCircles(img,cv2.HOUGH_GRADIENT,1,10,param1=50,param2=30,minRadius=0,maxRadius=20)
#if circles != None:
#    circles = np.uint16(np.around(circles))
#    for i in circles[0,:]:
#        # draw the outer circle
#        cv2.circle(cimg,(i[0],i[1]),i[2],(0,255,0),2)
#        # draw the center of the circle
#        cv2.circle(cimg,(i[0],i[1]),2,(0,0,255),3)

#    cv2.imshow(lastfile,cimg)
#    cv2.waitKey(0)
#    cv2.destroyAllWindows()

