package com.senabo.domain.stress.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QStress is a Querydsl query type for Stress
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QStress extends EntityPathBase<Stress> {

    private static final long serialVersionUID = -399506272L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QStress stress = new QStress("stress");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    public final NumberPath<Integer> changeAmount = createNumber("changeAmount", Integer.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final com.senabo.domain.member.entity.QMember memberId;

    public final NumberPath<Integer> score = createNumber("score", Integer.class);

    public final EnumPath<StressType> type = createEnum("type", StressType.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public QStress(String variable) {
        this(Stress.class, forVariable(variable), INITS);
    }

    public QStress(Path<? extends Stress> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QStress(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QStress(PathMetadata metadata, PathInits inits) {
        this(Stress.class, metadata, inits);
    }

    public QStress(Class<? extends Stress> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

