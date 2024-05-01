<template>
    <el-dialog
        v-model="visible"
        :title="form.title"
        width="800"
        draggable
        top="15vh"
        :close-on-click-modal="false"
        @open="dialogOnOpen"
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
                <el-form
                    v-if="form.mode === 'view'"
                    :model="entity"
                    label-width="auto"
                    label-position="top"
                    size="default"
                    :disabled="true"
                >
                    <div class="flex-row cg-4">
                        <div class="form-group fl-1">
                            <el-form-item label="Mã khóa luận">
                                <el-input v-model="entity.thesisCode" disabled />
                            </el-form-item>
                        </div>

                        <div class="form-group fl-4">
                            <el-form-item label="Tên đề tài dự kiến">
                                <el-input v-model="entity.thesisName" />
                            </el-form-item>
                        </div>
                    </div>
                    <div class="flex-row cg-4">
                        <div class="form-group fl-1">
                            <el-form-item label="Sinh viên">
                                <el-input
                                    v-model="entity.studentName"
                                    disabled
                                />
                            </el-form-item>
                        </div>

                        <div class="form-group fl-1">
                            <el-form-item label="Mã sinh viên">
                                <el-input
                                    v-model="entity.studentCode"
                                    disabled
                                />
                            </el-form-item>
                        </div>

                        <div class="form-group fl-1">
                            <el-form-item label="Khoa">
                                <el-input
                                    v-model="entity.facultyName"
                                />
                            </el-form-item>
                        </div>
                        <div class="form-group fl-1">
                            <el-form-item label="GPA">
                                <el-input
                                    v-model="student.gpa"
                                />
                            </el-form-item>
                        </div>
                    </div>

                    <div class="flex-row cg-4">
                        <div class="form-group fl-1">
                            <el-form-item label="Năm học">
                                <el-input v-model="entity.year" disabled/>
                            </el-form-item>
                        </div>

                        <div class="form-group fl-1">
                            <el-form-item label="Học kỳ">
                                <el-input v-model="entity.semester" disabled/>
                            </el-form-item>
                        </div>
                        <div class="form-group fl-1">
                            <el-form-item label="Trạng thái">
                                <el-input v-model="entity.status" disabled :formatter="() => ThesisStatus[entity.status]"/>
                            </el-form-item>
                        </div>
                    </div>
                    <div class="flex-row cg-4">
                        <div class="form-group fl-1">
                            <el-form-item label="Ghi chú">
                                <el-input
                                    v-model="entity.description"
                                    type="textarea"
                                    :rows="5"
                                />
                            </el-form-item>
                        </div>
                    </div>
                    <div class="flex-row cg-4">
                        <div class="form-group fl-1">
                            <el-form-item label="Giới thiệu sinh viên">
                                <el-input
                                    v-model="student.description"
                                    type="textarea"
                                    :rows="5"
                                />
                            </el-form-item>
                        </div>
                    </div>
                </el-form>
              
            </div>
        </div>

        <template #footer>
            <div class="dialog-footer">
                
                <el-button
                    type="primary"
                    @click="btnConfirmOnClick"
                    :loading="loading"
                >
                    Đồng ý
                </el-button>
                <el-button
                    type=""
                    @click="btnRefuseOnClick"
                    :loading="loading"
                >
                    Từ chối
                </el-button>
                <el-button @click="btnCancelOnClick">
                    Đóng
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref } from "vue";
import { useThesisStore, useStudentStore } from "@/stores";
import { storeToRefs } from "pinia";
import {ThesisStatus} from "@/common/enum";
import { ElMessage, ElMessageBox } from 'element-plus';
import {ThesisStatusEnum} from '@/common/enum';


const entityStore = useThesisStore();
const studentStore = useStudentStore();

const form = ref({
    title: "",
    mode: "",
    entityName: "Khoá luận",
});
const entity = ref({});
const student = ref({});
const listTeacher = ref([]);
const loadingGetTeacher = ref(false);

const props = defineProps({
    pEntityId: String,
    pMode: String,
});
const emit = defineEmits(["removeItem"]);

const { loading } = storeToRefs(entityStore);

const visible = defineModel("visible");

async function initData() {
    
    form.value.mode = props.pMode;
    form.value.title = "Thông tin khóa luận";

    // Get thesis info
    const e = await entityStore.getById(props.pEntityId);
    entity.value = { ...e };
    listTeacher.value = [{ userId: e.teacherId, teacherName: e.teacherName }];

    // Get student info
    const s = await studentStore.getById(e.studentId);
    if (s) {
        student.value = { ...s };
    }
}

async function btnConfirmOnClick() {
    
    ElMessageBox.confirm(
        'Đồng ý hướng dẫn khóa luận?',
        'Xác nhận',
        {
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Hủy',
        }
    )
    .then(async () => {
        entity.value.status = ThesisStatusEnum.ApprovedGuiding;
        let result = await entityStore.update({ ...entity.value });
        if (result) {
            closeFormAndEmitRemove();
        } else {
            ElMessage.error("Có lỗi xảy ra");
        }
    })
    .catch(() => {
    })
    
}

async function btnRefuseOnClick() {
    ElMessageBox.confirm(
        'Từ chối hướng dẫn khóa luận?',
        'Xác nhận',
        {
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Hủy',
        }
    )
    .then(async () => {
        entity.value.status = ThesisStatusEnum.RejectGuiding;
        let result = await entityStore.update({ ...entity.value });
        if (result) {
            closeFormAndEmitRemove();
        } else {
            ElMessage.error("Có lỗi xảy ra");
        }
    })
    .catch(() => {
    
    })
}


async function dialogOnOpen() {
    await initData();
}

function closeFormAndEmitRemove() {
    visible.value = false;
    emit("removeItem", props.pEntityId);
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

:deep(.el-select-dropdown__loading) {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100px;
  font-size: 20px;
}
:deep(.el-input__inner) {
    cursor: default !important;
}
</style>
