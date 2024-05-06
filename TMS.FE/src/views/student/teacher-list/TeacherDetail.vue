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
                            <el-form-item label="Mã giảng viên">
                                <el-input v-model="entity.teacherCode" disabled />
                            </el-form-item>
                        </div>

                        <div class="form-group fl-4">
                            <el-form-item label="Họ và tên">
                                <el-input v-model="entity.teacherName" disabled />
                            </el-form-item>
                        </div>
                    </div>
                    <div class="flex-row cg-4">
                        <div class="form-group fl-1">
                            <el-form-item label="Khoa">
                                <el-input v-model="entity.facultyName" disabled />
                            </el-form-item>
                        </div>

                        <div class="form-group fl-1">
                            <el-form-item label="Số điện thoại">
                                <el-input v-model="entity.phoneNumber"/>
                            </el-form-item>
                        </div>

                        <div class="form-group fl-1">
                            <el-form-item label="Địa chỉ email">
                                <el-input v-model="entity.email" />
                            </el-form-item>
                        </div>
                    </div>


                    <div class="form-group fl-1">
                        <el-form-item label="Hướng nghiên cứu">
                            <el-input
                                v-model="entity.description"
                                type="textarea"
                                :rows="8"
                            />
                        </el-form-item>
                    </div>
                </el-form>
            </div>
        </div>

        <template #footer>
            <div class="dialog-footer">
                <el-button @click="btnCancelOnClick">
                    Đóng
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref } from "vue";
import { useTeacherStore } from "@/stores";
import { storeToRefs } from "pinia";
import { ElMessage } from 'element-plus'

const entityStore = useTeacherStore();
const form = ref({
    title: '',
    mode: '',
    entityName: 'Giảng viên'
});
const entity = ref({});

const props = defineProps({
    pEntityId: String,
    pMode: String
});
const emit = defineEmits(['close']);

const {loading} = storeToRefs(entityStore)

const visible = defineModel("visible");

async function initData() {
    form.value.mode = props.pMode;
    form.value.title = 'Thông tin Giảng viên';
    const e = await entityStore.getById(props.pEntityId);
    entity.value = {...e};
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

:deep(.el-input__inner) {
    cursor: default !important;
}
:deep(.el-textarea__inner) {
    cursor: default !important;
}




</style>
