<template>
    <el-dialog
        v-model="visible"
        :title="form.title"
        width="800"
        draggable
        top="20vh"
        :close-on-click-modal="false"
        @open="dialogOnOpen"
    >
        <div
            class="dialog-body"
            v-loading.fullscreen="
                loading && visible
                    ? {
                          lock: true,
                          text: '',
                          background: 'rgba(0, 0, 0, 0.1)',
                      }
                    : false
            "
        >
            <div class="dialog-body">
                <el-form
                    :model="entity"
                    label-width="auto"
                    label-position="top"
                    size="default"
                    :disabled="form.mode === 'view'"
                >
                    <div class="flex-row cg-4">
                        <div class="form-group fl-1">
                            <el-form-item label="Mã khoa">
                                <el-input v-model="entity.facultyCode"/>
                            </el-form-item>
                        </div>

                        <div class="form-group fl-3">
                            <el-form-item label="Tên khoa">
                                <el-input v-model="entity.facultyName" />
                            </el-form-item>
                        </div>
                    </div>

                    <div class="form-group fl-1">
                        <el-form-item label="Mô tả">
                            <el-input 
                                v-model="entity.description" 
                                type="textarea"
                                :rows="4"
                            />
                        </el-form-item>
                    </div>
                </el-form>
            </div>
        </div>

        <template #footer>
            <div class="dialog-footer">
                <el-button @click="btnCancelOnClick">
                    {{ form.mode === 'view' ? 'Đóng' : 'Hủy' }}
                </el-button>
                <el-button
                    type="primary"
                    @click="btnConfirmOnClick"
                    :loading="loading"
                    v-if="form.mode !== 'view'"
                >
                    Xác nhận
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref } from "vue";
import { useFacultyStore } from "@/stores";
import { storeToRefs } from "pinia";
import { ElMessage } from 'element-plus'

const entityStore = useFacultyStore();
const form = ref({
    title: '',
    mode: '',
    entityName: 'Khoa'
});
const entity = ref({});

const props = defineProps({
    pEntityId: Object,
    pMode: String
});
const emit = defineEmits(['close']);

const {loading} = storeToRefs(entityStore)

const visible = defineModel("visible");

async function initData() {
    form.value.mode = props.pMode;

    if (form.value.mode === 'edit' || form.value.mode === 'view') {
        form.value.title = (form.value.mode === 'edit' ? 'Chỉnh sửa ' : 'Xem ') + form.value.entityName;
        const e = await entityStore.getById(props.pEntityId);
        entity.value = {...e};

    } else if (form.value.mode === 'add') {
        form.value.title = 'Thêm mới ' + form.value.entityName;
        const newEntity = await entityStore.getNew();
        entity.value = {...newEntity};
    }
}

async function btnConfirmOnClick() {
    let result = false;
    if (form.value.mode === 'add') {
        result = await entityStore.insert({...entity.value});
    } else {
        result = await entityStore.update({...entity.value});
    }
    if (result) {
        let message = form.value.mode === 'add' ? 'Thêm mới thành công' : 'Cập nhật thành công';
        ElMessage.success(message);
        gotoPageList();
    }
}

async function dialogOnOpen() {
    await initData();
}


function gotoPageList() {
    visible.value = false;
}

function btnCancelOnClick() {
    visible.value = false;
}
</script>

<style scoped>

:deep(.el-form-item__label) {
    min-width: 80px;
    justify-content: flex-start;
}




</style>
