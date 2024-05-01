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
                    v-if="form.mode === 'add' || form.mode === 'edit'"
                    :model="entity"
                    label-width="auto"
                    label-position="top"
                    size="default"
                >
                    <div class="flex-row cg-4">
                        <div class="form-group fl-1">
                            <el-form-item label="Mã khóa luận">
                                <el-input v-model="entity.thesisCode" disabled />
                            </el-form-item>
                        </div>

                        <div class="form-group fl-4">
                            <el-form-item label="Tên đề tài">
                                <el-input v-model="entity.thesisName" 
                                :disabled="entity.status === ThesisStatusEnum.ApprovedTitle"
                                />
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
                            <el-form-item label="Cán bộ hướng dẫn">
                                <el-select
                                    v-model="entity.teacherId"
                                    filterable
                                    remote
                                    reserve-keyword
                                    placeholder="Chọn cán bộ hướng dẫn"
                                    :remote-method="getListTeacher"
                                    :loading="loadingGetTeacher"
                                    remote-show-suffix
                                    :name="entity.teacherName"
                                    disabled
                                >
                                    <el-option
                                        v-for="teacher in listTeacher"
                                        :key="teacher.userId"
                                        :label="teacher.teacherName"
                                        :value="teacher.userId"
                                    />
                                    <template #loading>
                                        Loading
                                    </template>
                                </el-select>
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
                    </div>

                    <div class="form-group fl-1">
                        <el-form-item label="Ghi chú">
                            <el-input
                                v-model="entity.description"
                                type="textarea"
                                :rows="4"
                            />
                        </el-form-item>
                    </div>
                </el-form>
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
                            <el-form-item label="Tên đề tài">
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
                            <el-form-item label="Cán bộ hướng dẫn">
                                <el-input
                                    v-model="entity.teacherName"
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

                    <div class="form-group fl-1">
                        <el-form-item label="Ghi chú">
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
                    {{ form.mode === "view" ? "Đóng" : "Hủy" }}
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
import { useThesisStore, useAuthStore } from "@/stores";
import { storeToRefs } from "pinia";
import { ElMessage } from "element-plus";
import $api from "@/api/index.js";
import { httpClient } from "@/helpers";
import {ThesisStatus, ThesisStatusEnum} from "@/common/enum";



const entityStore = useThesisStore();

const form = ref({
    title: "",
    mode: "",
    entityName: "Khoá luận",
});
const entity = ref({});
const listTeacher = ref([]);
const loadingGetTeacher = ref(false);

const props = defineProps({
    pEntityId: String,
    pMode: String,
});
const emit = defineEmits(["close"]);

const { loading } = storeToRefs(entityStore);

const visible = defineModel("visible");

async function initData() {
    
    form.value.mode = props.pMode;

    if (form.value.mode === "edit" || form.value.mode === "view") {
        form.value.title =
            (form.value.mode === "edit" ? "Chỉnh sửa " : "Xem ") +
            form.value.entityName;
        const e = await entityStore.getById(props.pEntityId);
        entity.value = { ...e };

        listTeacher.value = [{ userId: e.teacherId, teacherName: e.teacherName }];

    } else if (form.value.mode === "add") {
        form.value.title = "Đăng ký " + form.value.entityName;
        const newEntity = await entityStore.getNew();
        entity.value = { ...newEntity };
    }
}

async function btnConfirmOnClick() {
    let result = false;
    if (form.value.mode === "add") {
        result = await entityStore.insert({ ...entity.value });
    } else {
        result = await entityStore.update({ ...entity.value });
    }
    if (result) {
        let message =
            form.value.mode === "add"
                ? "Thêm mới thành công"
                : "Cập nhật thành công";
        ElMessage.success(message);
        gotoPageList();
    }
}

async function getListTeacher(query) {
    loadingGetTeacher.value = true;
    await new Promise((resolve) => setTimeout(resolve, 500));
    const res = await httpClient.post($api.teacher.filter(), {
        skip: 0,
        take: 0,
        keySearch: query ?? '',
        filterColumns: ["teacherName", "phoneNumber", "email"],
    });
    if (res.data) {
        listTeacher.value = res.data;
    }
    loadingGetTeacher.value = false;
};

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

:deep(.el-select-dropdown__loading) {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100px;
  font-size: 20px;
}
</style>
