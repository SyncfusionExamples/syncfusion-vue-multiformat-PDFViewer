<template>
    <div>
        <div class="control-section">
            <div class="content-wrapper-pdfviewer">
                <div id="dropAreaRef" style="height: auto; overflow: auto">
                    <div class="browse-row">
                        <ejs-button id="browse" cssClass="e-primary browse-blue">Browse...</ejs-button>
                    </div>
                    <div>
                        <p style="margin: 15px;">OR</p>
                        <p id="drop">Drop files (Word, Excel, PowerPoint, Image, PDF)</p>
                    </div>
                </div>
                <div id="progressBar" style="display: none;">
                    <div id="fileDetails">
                        <p id="fileName"></p>
                        <p id="fileSize"></p>
                    </div>
                    <div id="linearProgressBar" style="justify-content: center; display: none;">
                        <ejs-progressbar type='Linear' id="linear-pdfviewer" ref="progressLinear" :value='value'
                            :animation="animation" :min="min" :max="max" :load="load" :height="height" :width="width">
                        </ejs-progressbar>
                        <span id="progress-status" style="padding: 18px 5px;"></span>
                    </div>
                    <div id="uploadedMessage" style="display: none; margin-top: 10px;">
                        <p style="color: rgb(110, 218, 110);">File successfully uploaded...</p>
                    </div>
                    <div id="FailedMessage" style="display: none; margin-top: 10px;">
                        <p style="color: red;">File not supported!</p>
                    </div>
                    <div id="fileSizeValidation" style="display: none; margin-top: 10px;">
                        <p style="color: rgb(203, 38, 38);">Maximum file size is (4.0 MB) for this operation...</p>
                    </div>
                </div>
                <div id="uploader-pdfviewer">
                    <ejs-uploader ref="uploadObj" id="fileUpload" :selected="onSelect" :allowedExtensions="extensions"
                        :dropArea="dropAreaRef" v-bind:multiple='false'></ejs-uploader>
                </div>
            </div>
            <div id="pdfviewer_container" style="display: none;">
                <ejs-pdfviewer id="pdfviewer" class="e-pv-multi-format-pdfviewer" ref="pdfviewer" :resourceUrl="resourceUrl"
                    :ajaxRequestSuccess="ajaxRequestSuccess" :toolbarSettings="toolbarSettings" :documentLoad="documentLoad"
                    :zoomMode="zoomMode">
                </ejs-pdfviewer>
            </div>
        </div>
    </div>
</template>

<style src="./App.css" />

<script>
import { UploaderComponent } from "@syncfusion/ej2-vue-inputs";
import {
    PdfViewerComponent, Toolbar, Magnification, Navigation, LinkAnnotation, BookmarkView,
    ThumbnailView, Print, TextSelection, TextSearch, Annotation, FormFields, FormDesigner,PageOrganizer
} from "@syncfusion/ej2-vue-pdfviewer";
import { isNullOrUndefined, createElement } from "@syncfusion/ej2-base";
import { ButtonComponent } from "@syncfusion/ej2-vue-buttons";
import { ProgressBarComponent } from "@syncfusion/ej2-vue-progressbar";

export default {
    components: {
        'ejs-pdfviewer': PdfViewerComponent,
        'ejs-uploader': UploaderComponent,
        'ejs-button': ButtonComponent,
        'ejs-progressbar': ProgressBarComponent
    },
    data: function () {
        return {
            resourceUrl: 'https://cdn.syncfusion.com/ej2/23.2.6/dist/ej2-pdfviewer-lib',
            toolbarSettings: {
                showTooltip: true, toolbarItems: ["DownloadOption",
                    "UndoRedoTool",
                    "PageNavigationTool",
                    "MagnificationTool",
                    "PanTool",
                    "SelectionTool",
                    "CommentTool",
                    "SubmitForm",
                    "SearchOption",
                    "AnnotationEditTool",
                    "FormDesignerEditTool",
                    "PrintOption"]
            },
            dropAreaRef: '.content-wrapper-pdfviewer',
            value: 0,
            min: 0,
            max: 100,
            height: '60',
            width: '250',
            extensions: '.doc, .docx, .rtf, .docm, .dotm, .dotx, .dot, .xls, .xlsx, .pptx, .pptm, .potx, .potm .jpeg, .png, .bmp, .pdf, .jpg',
            zoomMode: 'FitToPage',
            animation: {
                enable: false,
                delay: 0
            },
            pdfViewerProgressValue: 0,
            uploadProgressValue: 0,
        }
    },
    provide: {
        PdfViewer: [Toolbar, Magnification, Navigation, LinkAnnotation, BookmarkView, ThumbnailView, Print, TextSelection, TextSearch, Annotation, FormFields, FormDesigner,PageOrganizer]
    },
    mounted: function () {
        document.getElementById('browse').onclick = () => {
            document.getElementsByClassName('e-file-select-wrap')[0].querySelector('button').click();
            return false;
        }
    },
    methods: {
        onSelect: function (args) {
            var extensions = ['doc', 'docx', 'rtf', 'docm', 'dotm', 'dotx', 'dot', 'xls', 'xlsx', 'pptx', 'pptm', 'potx', 'potm', 'jpeg', 'png', 'bmp', 'pdf','jpg']
            let progress = this.$refs.progressLinear.ej2Instances;
            progress.value = 0;
            progress.refresh();
            document.getElementById("progress-status").innerHTML = "0%";
            let progressBarContainer = document.getElementById("progressBar");
            let progressBar = document.getElementById("linearProgressBar");
            let progressMessage = document.getElementById("uploadedMessage");
            let fileSizeValidation = document.getElementById("fileSizeValidation");
            document.getElementById("fileDetails").style.display = "block";
            document.getElementById("FailedMessage").style.display = "none";
            progressBarContainer.style.display = "block";
            progressBar.style.display = "flex";
            progressMessage.style.display = "none";
            fileSizeValidation.style.display = "none";
            if (isNullOrUndefined(this.$refs.uploadObj.$el.querySelector('.e-upload-files'))) {
                var parentElement = createElement('ul', {
                    className: 'e-upload-files',
                });
                document.getElementsByClassName('e-upload')[0].appendChild(parentElement);
            }
            var validFiles = args.filesData;
            if (validFiles.length === 0) {
                progressBarContainer.style.display = "block";
                progressBar.style.display = "none";
                progressMessage.style.display = "block";
                args.cancel = true;
                return;
            }
            if (!extensions.includes(validFiles[0].type)) {
                document.getElementById("FailedMessage").style.display = "block";
                document.getElementById("fileDetails").style.display = "none"
                progressBar.style.display = "none";
                progressMessage.style.display = "none";
                document.getElementById('pdfviewer_container').style.display = 'none';
                args.cancel = true;
                return;
            }
            if (validFiles[0].type != "pdf" && validFiles[0].size > 4000000) {
                fileSizeValidation.style.display = "block";
                progressBar.style.display = "none";
                document.getElementById("fileDetails").style.display = "none";
                document.getElementById('pdfviewer_container').style.display = 'none';
                args.cancel = true;
                return;
            }
            document.getElementById("fileName").innerHTML = args.filesData[0].name;
            var viewer = document.getElementById('pdfviewer').ej2_instances[0];
            viewer.downloadFileName = args.filesData[0].name;
            viewer.exportAnnotationFileName = args.filesData[0].name;
            let size = document.getElementById("fileSize");
            if ((args.filesData[0].size.toString()).length <= 7) {
                size.innerHTML = ((args.filesData[0].size / 1024).toFixed(1)).toString() + " KB";
            } else {
                let kbsize = args.filesData[0].size / 1024;
                size.innerHTML = ((kbsize / 1024).toFixed(1)).toString() + " MB";
            }
            document.getElementById("fileSize");
            this.formSelectedData(validFiles[0], this.$refs.uploadObj);
            const totalProgress = this.calculateTotalProgress();
            this.updateProgressBar(totalProgress);
            document.getElementById("progress-status").innerHTML = totalProgress.toString() + "%";
        },
        formSelectedData: function (file, proxy) {
            var liEle = createElement('li', {
                className: 'e-upload-file-list',
                attrs: {
                    'data-file-name': file.name
                },
            });
            this.readURL(liEle, file);
            proxy.ej2Instances.fileList.push(liEle);
        },
        readURL: function (li, args) {
            var file = args.rawFile;
            var reader = new FileReader();
            var type = args.type;
            const data = this;
            reader.addEventListener('load', function () {
                let post = JSON.stringify({
                    'data': reader.result,
                    'type': type
                })
                const url = "http://localhost:5007/PdfViewer/LoadFile"
                let xhr = new XMLHttpRequest()
                xhr.open('Post', url, true)
                xhr.setRequestHeader('Content-type', 'application/json; charset=UTF-8')
                xhr.upload.addEventListener('progress', (event) => {
                    if (event.lengthComputable) {
                        const progressValue = Math.round((event.loaded / event.total) * 100);
                        data.uploadProgressValue = progressValue;
                        const totalProgress = data.calculateTotalProgress();
                        data.updateProgressBar(totalProgress);
                        document.getElementById("progress-status").innerHTML = totalProgress.toString() + "%";
                    }
                });
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === XMLHttpRequest.DONE) {
                        if (xhr.status === 200) {
                            var viewer = document.getElementById('pdfviewer').ej2_instances[0];
                            viewer.documentPath = '';
                            viewer.documentPath = xhr.responseText;
                            data.pdfViewerProgressValue = 20;
                            const totalProgress = data.calculateTotalProgress();
                            data.updateProgressBar(totalProgress);
                            document.getElementById("progress-status").innerHTML = totalProgress.toString() + "%";
                            document.getElementById('pdfviewer_container').style.display = 'block';
                        } else {
                            console.error('Error:', xhr.statusText);
                        }
                    }
                }.bind(this);
                xhr.send(post);
            }, false);
            if (file) {
                reader.readAsDataURL(file);
            }
        },
        ajaxRequestSuccess: function (args) {
            if (args.action === "Load") {
                this.pdfViewerProgressValue = 50;
                const totalProgress = this.calculateTotalProgress();
                this.updateProgressBar(totalProgress);
                document.getElementById("progress-status").innerHTML = totalProgress.toString() + "%";
            }
        },
        documentLoad: function () {
            this.pdfViewerProgressValue = 100;
            const totalProgress = this.calculateTotalProgress();
            this.updateProgressBar(totalProgress);
            document.getElementById("progress-status").innerHTML = totalProgress.toString() + "%";
            setTimeout(() => {
                document.getElementById("linearProgressBar").style.display = "none";
                document.getElementById("uploadedMessage").style.display = "block";
                this.uploadProgressValue = 0;
                this.pdfViewerProgressValue = 0;
                let progress = this.$refs.progressLinear.ej2Instances;
                progress.value = 0;
            }, 1000);
        },
        calculateTotalProgress: function () {
            const totalProgress = (this.uploadProgressValue + this.pdfViewerProgressValue) / 2;
            return totalProgress;
        },
        updateProgressBar: function (totalprogress) {
            let progress = this.$refs.progressLinear.ej2Instances;
            if (progress) {
                progress.value = totalprogress;
                progress.dataBind();
            }
        },
        load: function (args) {
            let selectedTheme = location.hash.split('/')[1];
            selectedTheme = selectedTheme ? selectedTheme : 'Material';
            args.progressBar.theme = (selectedTheme.charAt(0).toUpperCase() +
                selectedTheme.slice(1)).replace(/-dark/i, 'Dark').replace(/contrast/i, 'Contrast');
            if (selectedTheme === 'material') {
                args.progressBar.secondaryProgressColor = '#f087ab'
            }
        },
    }
};
</script>