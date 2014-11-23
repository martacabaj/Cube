#-------------------------------------------------
#
# Project created by QtCreator 2014-11-23T14:12:36
#
#-------------------------------------------------

QT       += core
QT       -= gui

TARGET = rubik
CONFIG   += console
CONFIG   -= app_bundle

TEMPLATE = app


SOURCES += main.cpp

QMAKE_CXXFLAGS += -std=c++11
QMAKE_MACOSX_DEPLOYMENT_TARGET = 10.9

LIBS += -L/usr/local/lib


win32:CONFIG(release, debug|release): LIBS += \
    -lopencv_core290 \
    -lopencv_highgui290 \
    -lopencv_video290 \
    -lopencv_ml290 \
    -lopencv_imgproc290 \
    -lopencv_legacy290 \
    -lopencv_objdetect290 \
    -lopencv_softcascade290 \
    -llibboost_log-vc100-mt-1_55
else:win32:CONFIG(debug, debug|release): LIBS +=  \
    -lopencv_core290d \
    -lopencv_highgui290d \
    -lopencv_video290d \
    -lopencv_ml290d \
    -lopencv_imgproc290d \
    -lopencv_legacy290d \
    -lopencv_objdetect290d \
    -lopencv_softcascade290d \
    -llibboost_log-vc100-mt-gd-1_55
else:unix: LIBS += \
    -lopencv_calib3d \
    -lopencv_contrib \
    -lopencv_core \
    -lopencv_flann \
    -lopencv_gpu \
    -lopencv_highgui \
    -lopencv_imgproc \
    -lopencv_legacy \
    -lopencv_ml \
    -lopencv_objdetect \
    -lopencv_ocl \
    -lopencv_photo \
    -lopencv_stitching \
    -lopencv_superres \
    -lopencv_ts \
    -lopencv_video \
    -lopencv_videostab
#    -lopencv_nonfree


INCLUDEPATH += /usr/local/include/opencv \
                /usr/local/include/opencv2 \
                   /usr/include/opencv \
                /usr/local/include

DEPENDPATH += /usr/local/incl
