<template>
    <el-dialog 
        v-model="dialogVisible" 
        :title="form.title" 
        width="800" draggable 
        :close-on-click-modal="false"
        @close="dialogOnClose"
        >  
        <div class="dialog-body"
            v-loading.fullscreen="loading ? {
                lock: true,
                text: '',
                background: 'rgba(0, 0, 0, 0.1)',
            } : false"
        >
            <el-form
                :model="entity"
                label-width="auto"
                label-position="top"
                size="default"
                :disabled="form.mode === 'view'"
            >
                <div class="subtitle">Thông tin chung</div>
                <div class="flex-row cg-4">
                    <div class="form-group">
                        <el-form-item label="Mã khách hàng">
                            <el-input v-model="entity.tenantCode" />
                        </el-form-item>
                    </div>

                    <div class="form-group fl-1">
                        <el-form-item label="Tên khách hàng">
                            <el-input v-model="entity.tenantName" />
                        </el-form-item>
                    </div>
                </div>
                <div class="flex-row cg-4">
                    <div class="form-group fl-3">
                        <el-form-item label="Địa chỉ">
                            <el-input v-model="entity.address" />
                        </el-form-item>
                    </div>

                    <div class="form-group fl-1">
                        <el-form-item label="Số điện thoại">
                            <el-input v-model="entity.phoneNumber" />
                        </el-form-item>
                    </div>
                    <div class="form-group fl-2">
                        <el-form-item label="Email">
                            <el-input v-model="entity.email" />
                        </el-form-item>
                    </div>
                </div>
                <div class="flex-row cg-4">
                    <div class="form-group fl-2">
                        <el-form-item label="Tên người đại diện">
                            <el-input v-model="entity.surrogateName" />
                        </el-form-item>
                    </div>

                    <div class="form-group fl-1">
                        <el-form-item label="Số điện thoại NĐD">
                            <el-input v-model="entity.surrogatePhoneNumber" />
                        </el-form-item>
                    </div>
                    <div class="form-group fl-2">
                        <el-form-item label="Email NĐD">
                            <el-input v-model="entity.surrogateEmail" />
                        </el-form-item>
                    </div>
                </div>
                <div class="subtitle">Thiết lập</div>
                <div class="flex-row cg-4">
                    <div class="form-group">
                        <el-form-item label="Tự động tạo cơ sở dữ liệu">
                            <el-switch v-model="entity.autoCreateDB" />
                        </el-form-item>
                    </div>
                    <div class="form-group fl-1" v-if="!entity.autoCreateDB">
                        <el-form-item label="Connection string">
                            <el-input v-model="entity.dBConnection" />
                        </el-form-item>
                    </div>
                </div>
                <div class="flex-row">
                    <div class="form-group fl-1">
                        <el-form-item label="Tên Domain">
                            <el-input v-model="entity.domain" />
                        </el-form-item>
                    </div>
                </div>
                
            </el-form>

        </div>

        <template #footer>
            <div class="dialog-footer">
                <div class="form-add-edit-footer" v-if="form.mode !== 'view'">
                    <el-button @click="btnCancelOnClick">Hủy</el-button>
                    <el-button type="primary" @click="btnConfirmOnClick">
                        Đồng ý
                    </el-button>
                </div>
                <div v-else class="form-view-footer">
                    <el-button @click="btnCloseOnClick">Đóng</el-button>
                </div>
            </div>
        </template>
    </el-dialog>
</template>

<script setup>
    import { ref } from 'vue';
    import { useRouter, useRoute } from 'vue-router';
    import {useTenantStore} from '@/stores';
    import {useForm} from '@/composables'
    import { storeToRefs } from 'pinia';

    const router = useRouter();
    const route = useRoute();
    const tenantStore = useTenantStore();

    const {loading} = storeToRefs(tenantStore);

    const dialogVisible = ref(true);

    const {form, entity} = useForm('Khách hàng');


    initData();

    async function initData() {
        if (form.value.mode === 'edit' || form.value.mode === 'view') {
            const tenantId = route.params.id;
            const tenant = await tenantStore.getById(tenantId);
            entity.value = {...tenant};
        } else if (form.value.mode === 'add') {
            const newEntity = await tenantStore.getNew();
            entity.value = {...newEntity};
        }
    }

    function gotoPageList() {
        router.push('/tenant');
    }

    async function btnConfirmOnClick() {
        if (form.value.mode === 'add') {
            await tenantStore.insert({...entity.value});
        } else {
            await tenantStore.update({...entity.value});
        }
    }

    function btnCloseOnClick() {
        gotoPageList();
    }

    function dialogOnClose() {
        gotoPageList();
    }

    function btnCancelOnClick() {
        gotoPageList();
    }



</script>

<style scoped>
    .subtitle {
        font-weight: bold;
        font-size: 16px;
        margin-bottom: 8px;
        margin-top: 8px;
    }

</style>