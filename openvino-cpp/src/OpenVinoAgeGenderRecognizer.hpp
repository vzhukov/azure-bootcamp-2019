#pragma once

#include <gflags/gflags.h>
#include <functional>
#include <iostream>
#include <fstream>
#include <memory>
#include <chrono>
#include <string>
#include <utility>
#include <algorithm>
#include <iterator>
#include <inference_engine.hpp>
#include <ie_iextension.h>
#include <opencv2/opencv.hpp>
#include "common.hpp"

using namespace InferenceEngine;
using namespace cv;
using namespace std;

struct AgeGenderResult {
	float age;
	bool isMale;
};

class OpenVinoAgeGenderRecognizer {
public:
	OpenVinoAgeGenderRecognizer() {
	}

	OpenVinoAgeGenderRecognizer(InferencePlugin& plugin) {
		map<string, string> config;
		network = plugin.LoadNetwork(InitNetwork(), config);
	}

	CNNNetwork InitNetwork() {
		CNNNetReader netReader;
		netReader.ReadNetwork(modelXml);
		netReader.getNetwork().setBatchSize(1);
		netReader.ReadWeights(modelBin);
		InputsDataMap inputInfo(netReader.getNetwork().getInputsInfo());
		InputInfo::Ptr& inputInfoFirst = inputInfo.begin()->second;
		inputInfoFirst->setPrecision(Precision::U8);
		input = inputInfo.begin()->first;
		OutputsDataMap outputInfo(netReader.getNetwork().getOutputsInfo());
		auto it = outputInfo.begin();

		DataPtr ptrAgeOutput = (it++)->second;
		DataPtr ptrGenderOutput = (it++)->second;
		auto genderCreatorLayer = ptrGenderOutput->getCreatorLayer().lock();
		auto ageCreatorLayer = ptrAgeOutput->getCreatorLayer().lock();
		if (genderCreatorLayer->type == "Convolution") {
			std::swap(ptrAgeOutput, ptrGenderOutput);
		}
		outputAge = ptrAgeOutput->name;
		outputGender = ptrGenderOutput->name;
		return netReader.getNetwork();

	}
	string input;
	ExecutableNetwork network;
	InferRequest::Ptr request;
	string inputName;

	string outputAge;
	string outputGender;

	AgeGenderResult GetResult(Mat& frame) {
		AgeGenderResult res;
		if (!request) {
			request = network.CreateInferRequestPtr();
		}
		//request->SetBatch(1);
		frameToBlob(frame, request, input);
		request->Infer();
		Blob::Ptr genderBlob = request->GetBlob(outputGender);
		Blob::Ptr ageBlob = request->GetBlob(outputAge);
		res.age = ageBlob->buffer().as<float*>()[0] * 100;
		res.isMale = genderBlob->buffer().as<float*>()[1] > 0.5;
		return res;
	}


private:
	string modelBase = "./models/age-gender-recognition-retail-0013";
	string modelXml = modelBase + ".xml";
	string modelBin = modelBase + ".bin";
};
