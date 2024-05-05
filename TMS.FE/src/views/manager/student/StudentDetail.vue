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
            <div class="dialog-body">

            </div>
        </div>

        <template #footer>
            <div class="dialog-footer">
                <el-button @click="btnCancelOnClick">Hủy</el-button>
                <el-button
                    type="primary"
                    @click="btnConfirmOnClick"
                    :loading="loading"
                >
                    Xác nhận
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref } from "vue";
const props = defineProps({});
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


async function initData() {}

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
