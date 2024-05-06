<template>
    <el-dialog
        v-model="visible"
        :title="form.title"
        width="800"
        draggable
        :close-on-click-modal="false"
        @close="dialogOnClose"
    >
        <div
            class="dialog-body"
            v-loading.fullscreen="
                loading
                    ? {
                          lock: true,
                          text: '',
                          background: 'rgba(0, 0, 0, 0.1)',
                      }
                    : false
            "
        >
            <div class="upload-container" v-if="form.mode === 'upload'">
                <div class="upload-instruction">
                    Tải tệp dữ liệu mẫu 
                    <span class="download_sample_btn" @click="downloadSampleFile">Tải về</span>
                </div>
                <el-upload
                    class="upload"
                    drag
                    :action="pUrlUpload"
                    multiple
                    :auto-upload="false"
                    :limit="1"
                    ref="refUpload"
                    :on-success="handleOnSuccess"
                >
                    <el-icon class="el-icon--upload"><upload-filled /></el-icon>
                    <div class="el-upload__text">
                        Kéo tệp vào đây hoặc <em>click để tải lên</em>
                    </div>
                    <template #tip>
                        <div class="el-upload__tip">Tệp tải lên:</div>
                    </template>
                </el-upload>
            </div>
            <div v-else class="result-container">
                <div class="subtitle">Kết quả nhập khẩu dữ liệu</div>
                <div class="import-success" v-show="form.mode === 'success'">
                    <div class="success-message">
                        <div class="line">Nhập khẩu thành công {{ form.rowsSuccess }} bản ghi.</div>   
                    </div>
                </div>
                <div class="import-failed" v-show="form.mode === 'failed'">
                    <div class="error-message">
                        <div class="line">Nhập khẩu thất bại, vui lòng kiểm tra lại tệp dữ liệu.</div>
                        <div class="line">Số dòng không hợp lệ: {{ form.rowsError.length }}</div>    
                    </div>
                    <el-table
                        height="280"
                        :data="form.rowsError"
                        style="width: 100%"
                        row-key="rowIndex"
                        v-loading="loading"
                    >
                        <el-table-column prop="rowIndex" label="Dòng" />
                        <el-table-column prop="errorMessage" label="Trạng thái" />
                    </el-table>
                </div>
            </div>
        </div>

        <template #footer>
            <div class="dialog-footer">
                <div class="upload-mode" v-show="form.mode === 'upload'">
                    <el-button @click="btnCancelOnClick">Hủy</el-button>
                    <el-button type="primary" @click="btnConfirmOnClick">
                        Đồng ý
                    </el-button>
                </div>
                <div class="success-mode" v-show="form.mode === 'success'">
                    <el-button @click="btnCancelOnClick">Đóng</el-button>
                </div>
                <div class="error-mode" v-show="form.mode === 'failed'">
                    <el-button @click="btnCancelOnClick">Hủy</el-button>
                    <el-button type="primary" @click="btnTryAgainOnClick">
                        Thử lại
                    </el-button>
                </div>
            </div>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref } from "vue";
import { UploadFilled } from "@element-plus/icons-vue";
const props = defineProps({
    pUrlUpload: String,
    pUrlDownloadSample: String,
});

const emit = defineEmits(['close']);

const refUpload = ref(null);
const loading = ref(false);
const form = ref({
    title: "Tải lên dữ liệu",
    mode: "upload", // "upload" "success" "failed",
    rowsSuccess: 0,
    rowsError: []
});

const visible = defineModel("visible");

initData();

async function initData() {}

async function downloadSampleFile() { 
    // download file from pUrlDownloadSample url
    const response = await fetch(props.pUrlDownloadSample);
    if (!response.ok) {
        throw new Error("Network response was not ok");
    }
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = "Mau_nhap_khau.xlsx"; // or any other filename you want
    document.body.appendChild(a); // we need to append the element to the dom -> otherwise it will not work in firefox
    a.click();    
    a.remove();  //afterwards we remove the element again         
}

function btnTryAgainOnClick() {
    resetForm();
}

async function btnConfirmOnClick() {
    await refUpload.value.submit();
}

function handleOnSuccess(response) {
    console.log(response);
    if (response.success) {
        form.value.mode = "success";
        form.value.rowsSuccess = response.data.rowsSuccess;
    } else {
        form.value.mode = "failed";
        form.value.rowsError = response.data.rowsError;
    }
}

function resetForm() {
    if (refUpload.value) {
        refUpload.value.clearFiles();
    }
    form.value.mode = "upload";
    form.value.rowsSuccess = 0;
    form.value.rowsError = [];
}


function dialogOnClose() {
    emit('close', form.value.rowsSuccess > 0);
    resetForm();
}

function btnCancelOnClick() {
    visible.value = false;
}
</script>

<style scoped>

.download_sample_btn {
    color: #1890ff;
    cursor: pointer;
}


.subtitle {
    font-weight: bold;
    text-align: center;
    font-size: 14px;
    margin-bottom: 8px;
    margin-top: 8px;
}
.upload-instruction {
    margin-bottom: 20px;
}
.message {
    padding: 10px;
    border-radius: 5px;
    margin-bottom: 10px;
    color: #333;
    font-size: 16px;
}

.import-success {
    height: 80px;
}

.error-message {
    background-color: #f8d7da;
    padding: 12px;
    border-color: #f5c6cb;
    color: #721c24;
}

.success-message {
  background-color: #d4edda;
  padding: 12px;
  border-color: #c3e6cb;
  color: #155724;
}



</style>
